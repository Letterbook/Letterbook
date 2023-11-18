using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Adapter.ActivityPub.Mappers;
using Letterbook.Core.Adapters;
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
    private Models.Profile? _profile = default;
    
    private static readonly IMapper ProfileMapper = new Mapper(ProfileMappers.DefaultProfile);
    private static readonly IMapper AsApMapper = new Mapper(Mappers.AsApMapper.Config);

    public Client(ILogger<Client> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }
    
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    // Stream saves on allocations
    private async Task<Stream?> ReadResponse(HttpResponseMessage response, 
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
                return default;
            case >= 201 and < 300:
                return default;
            case 200:
                return await response.Content.ReadAsStreamAsync();
            default:
                return default;
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

    public async Task<object> SendAccept(Uri inbox, IContentRef content)
    {
        throw new NotImplementedException();
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
    }
}