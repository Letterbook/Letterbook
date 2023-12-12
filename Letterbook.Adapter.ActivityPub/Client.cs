using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.Conversion;
using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Adapter.ActivityPub.Mappers;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Values;
using Microsoft.Extensions.Logging;
// TODO: Remove after fully implemented
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Letterbook.Adapter.ActivityPub;

public class Client : IActivityPubClient, IActivityPubAuthenticatedClient, IDisposable
{
    private readonly ILogger<Client> _logger;
    private readonly HttpClient _httpClient;
    private readonly IJsonLdSerializer _jsonLdSerializer;
    private readonly JsonLdSerializerOptions _jsonLdSerializerOptions;
    private Models.Profile? _profile = default;
    
    private static readonly IMapper ProfileMapper = new Mapper(ProfileMappers.DefaultProfile);
    private static readonly IMapper AsApMapper = new Mapper(Mappers.AsApMapper.Config);
    private static readonly IMapper ToLinkMapper = new Mapper(ProfileMappers.DefaultLink);

    public Client(ILogger<Client> logger, HttpClient httpClient, IJsonLdSerializer jsonLdSerializer)
    {
        _logger = logger;
        _httpClient = httpClient;
        _jsonLdSerializer = jsonLdSerializer;
    }
    
    // Using Stream instead of string or bytes saves on memory allocations
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    private async Task<Stream?> ReadResponse(HttpResponseMessage response, 
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        if(await ValidateResponseHeaders(response, name, path, line))
            return await response.Content.ReadAsStreamAsync();
        return default;
    }

    // ValueTask is more efficient when you expect to frequently just return a synchronous value
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    private async ValueTask<bool> ValidateResponseHeaders(HttpResponseMessage response,
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        switch ((int)response.StatusCode)
        {
            case >= 500:
                _logger.LogInformation("Received server error from {Uri}", response.RequestMessage?.RequestUri);
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Server response had headers {Headers} and body {Body}", response.Headers,
                        await response.Content.ReadAsStringAsync());
                throw ClientException.RemoteHostError(response.StatusCode, name: name, path: path, line: line);
            case >= 400 and < 500:
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received client error from {Uri}", response.RequestMessage?.RequestUri);
                _logger.LogDebug("Server response had headers {Headers} and body {Body}", response.Headers, body);
                throw ClientException.RequestError(response.StatusCode, name, body: body, path: path, line: line);
            case >= 300 and < 400:
                return false;
            case >= 201 and < 300:
                return default;
            case 200:
                return true;
            default:
                return false;
        }

    }
    
    public IActivityPubAuthenticatedClient As(Models.Profile? onBehalfOf)
    {
        if (onBehalfOf == null) return this;
        
        _profile = onBehalfOf;
        
        return this;
    }

    private HttpRequestMessage SignedRequest(HttpMethod method, Uri uri)
    {
        var message = new HttpRequestMessage(method, uri);
        if (_profile == null) return message;
        
        var httpRequestOptionsKey = new HttpRequestOptionsKey<IEnumerable<SigningKey>>(IClientSigner.SigningKeysOptionsId);
        var httpRequestOptionsProfile = new HttpRequestOptionsKey<Uri>(IClientSigner.ProfileOptionsId);
        message.Options.Set(httpRequestOptionsKey, _profile.Keys);
        message.Options.Set(httpRequestOptionsProfile, _profile.Id);

        return message;
    }

    public async Task<FollowState> SendFollow(Models.Profile target)
    {
        var inbox = target.Inbox;
        var actor = ProfileMapper.Map<AsAp.Actor>(_profile);
        var follow = new AsAp.Activity()
        {
            Type = "Follow",
        };
        follow.Actor.Add(actor);
        // Interop(mastodon): Mastodon requires the target to be in the Follow.Subject, even though the activity is 
        // about to be delivered to the target's Inbox
        follow.Object.Add(new AsAp.Link(CompactIri.FromUri(target.Id)));
        
        var message = SignedRequest(HttpMethod.Post, inbox);
        message.Content = JsonContent.Create(follow, options: JsonOptions.ActivityPub);

        _logger.LogDebug("Sending {Activity}", JsonSerializer.Serialize(follow, JsonOptions.ActivityPub));
        var response = await _httpClient.SendAsync(message);
        
        var stream = await ReadResponse(response);
        if (stream == null)
        {
            return FollowState.Pending;
        }
        try
        {
            var responseActivity = await JsonSerializer.DeserializeAsync<AsAp.Activity>(stream, JsonOptions.ActivityPub);
            if (responseActivity == null) throw new NotImplementedException();

            if (responseActivity.Type.Equals("Accept", StringComparison.InvariantCultureIgnoreCase))
                return FollowState.Accepted;
        
            if (responseActivity.Type.Equals("Reject", StringComparison.InvariantCultureIgnoreCase))
                return FollowState.Rejected;
        
            var isPending = responseActivity.Type.Equals("PendingAccept", StringComparison.InvariantCultureIgnoreCase);
            var isRejected = responseActivity.Type.Equals("PendingReject", StringComparison.InvariantCultureIgnoreCase);
            if (isPending || isRejected)
                return FollowState.Pending;

            _logger.LogWarning("Unrecognized response to {Activity}: {ResponseActivity}", nameof(SendFollow), responseActivity.Type);
            _logger.LogDebug("Unrecognized response to {Activity}: {@ResponseActivity}", nameof(SendFollow), responseActivity);
            return FollowState.None;
        }
        catch (JsonException e)
        {
            _logger.LogError("Invalid response to {Activity}: {Message}", nameof(SendFollow), e.Message);
            return FollowState.None;
        }
    }

    public async Task<object> SendCreate(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendUpdate(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendDelete(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendBlock(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendBoost(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendLike(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendDislike(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendAccept(Uri inbox, ActivityType activityToAccept, Uri requestorId, Uri? subjectId)
    {
        /*** Mastodon expects objects that look like this
         * {
         *   '@context': 'https://www.w3.org/ns/activitystreams',
         *   id: 'foo',
         *   type: 'Accept',
         *   actor: 'https://letterbook.example/actor/me',
         *   object: {
         *     id: 'bar',
         *     type: 'Follow',
         *     actor: 'https://mastodon.example/user/them',
         *     object: 'https://letterbook.example/actor/me',
         *   }
         */
        if (_profile is null || subjectId is null)
            throw CoreException.MissingData("Cannot build a semantic Accept Activity without an Actor and Object",
                typeof(Models.Profile), null);
        ASActivity acceptObject = BuildActivity(activityToAccept);
        var accept = BuildActivity(ActivityType.Accept, _profile, acceptObject);

        acceptObject.Actor.Add(requestorId);
        acceptObject.Object.Add(_profile.Id);
        
        return SendAccept(inbox, accept);
    }

    private async Task SendAccept(Uri inbox, AsAp.Activity activity)
    {
        var message = SignedRequest(HttpMethod.Post, inbox);
        message.Content = JsonContent.Create(activity, options: JsonOptions.ActivityPub);
        
        _logger.LogDebug("Sending {Activity}", JsonSerializer.Serialize(activity, JsonOptions.ActivityPub));
        var response = await _httpClient.SendAsync(message);
        
        await ValidateResponseHeaders(response);
    }

    private async Task SendAccept(Uri inbox, ASActivity activity)
    {
        var message = SignedRequest(HttpMethod.Post, inbox);
        var payload = _jsonLdSerializer.Serialize(activity);
        message.Content = new StringContent(payload, Constants.LdJsonHeader);
        
        _logger.LogDebug("Sending {Activity}", payload);
        var response = await _httpClient.SendAsync(message);
        
        await ValidateResponseHeaders(response);
    }

    public async Task<object> SendReject(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendPending(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendAdd(Uri inbox, IContentRef content, Uri collection)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendRemove(Uri inbox, IContentRef content, Uri collection)
    {
        throw new NotImplementedException();
    }

    public async Task<object> SendUnfollow(Uri inbox)
    {
        throw new NotImplementedException();
    }

    public async Task<T> Fetch<T>(Uri id) where T : IObjectRef
    {
        var response = await _httpClient.SendAsync(SignedRequest(HttpMethod.Get, id));
        var stream = await ReadResponse(response);
        var fetchType = typeof(T).IsAssignableFrom(typeof(Models.Profile)) ? typeof(AsAp.Actor) : typeof(AsAp.Object);
        var obj = await JsonSerializer.DeserializeAsync(stream, fetchType, JsonOptions.ActivityPub);

        var mapped = AsApMapper.Map<T>(obj);
        if (mapped.Id == id) return mapped;
        _logger.LogError("Remote fetch for Object {Id} returned {ObjectId}", id, mapped.Id);
        throw ClientException.RemoteObjectError(mapped.Id, "Peer provided object is not the same as requested");
    }

    private ASActivity BuildActivity(ActivityType type)
    {
        return type switch
        {
            ActivityType.Accept => new AcceptActivity(),
            ActivityType.Add => new AddActivity(),
            ActivityType.Announce => new AnnounceActivity(),
            ActivityType.Arrive => new ArriveActivity(),
            ActivityType.Block => new BlockActivity(),
            ActivityType.Create => new CreateActivity(),
            ActivityType.Delete => new DeleteActivity(),
            ActivityType.Dislike => new DislikeActivity(),
            ActivityType.Flag => new FlagActivity(),
            ActivityType.Follow => new FollowActivity(),
            ActivityType.Ignore => new IgnoreActivity(),
            ActivityType.Invite => new InviteActivity(),
            ActivityType.Join => new JoinActivity(),
            ActivityType.Leave => new LeaveActivity(),
            ActivityType.Like => new LikeActivity(),
            ActivityType.Listen => new ListenActivity(),
            ActivityType.Move => new MoveActivity(),
            ActivityType.Offer => new OfferActivity(),
            ActivityType.Question => new QuestionActivity(),
            ActivityType.Reject => new RejectActivity(),
            ActivityType.Read => new ReadActivity(),
            ActivityType.Remove => new RemoveActivity(),
            ActivityType.TentativeReject => new TentativeRejectActivity(),
            ActivityType.TentativeAccept => new TentativeAcceptActivity(),
            ActivityType.Travel => new TravelActivity(),
            ActivityType.Undo => new UndoActivity(),
            ActivityType.Update => new UpdateActivity(),
            ActivityType.View => new ViewActivity(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private ASActivity BuildActivity(ActivityType type, Models.Profile actor)
    {
        var activity = BuildActivity(type);
        activity.Actor.Add(actor.Id);

        return activity;
    }

    private ASActivity BuildActivity(ActivityType type, Models.Profile actor, ASObject @object)
    {
        var activity = BuildActivity(type, actor);
        activity.Object.Add(@object);

        return activity;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
    }
}