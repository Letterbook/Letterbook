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

namespace Letterbook.Adapter.ActivityPub;

public class Client : IActivityPubClient, IActivityPubAuthenticatedClient, IDisposable
{
    private readonly ILogger<Client> _logger;
    private readonly HttpClient _httpClient;
    private Models.Profile? _profile = default;
    private IMapper _profileMapper = new Mapper(ProfileMappers.DefaultProfile);
    private KeyContainer _keys;

    public Client(ILogger<Client> logger, HttpClient httpClient, KeyContainer keys)
    {
        _logger = logger;
        _httpClient = httpClient;
        _keys = keys;
    }
    
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    private static async Task<Stream?> ReadResponse(HttpResponseMessage response, 
        [CallerMemberName] string name="",
        [CallerFilePath] string path="",
        [CallerLineNumber] int line=-1)
    {
        return (int)response.StatusCode switch
        {
            >= 500 => throw ClientException.RemoteHostError(response.StatusCode, name: name, path: path, line: line),
            >= 400 and < 500 => throw ClientException.RequestError(response.StatusCode, name: name, path: path, line: line),
            >= 300 and < 400 => default, // TODO: handle redirects
            >= 201 and < 300 => default, // TODO: 2xx
            200 => await response.Content.ReadAsStreamAsync(),
            < 200 => default,
        };
    }
    
    public IActivityPubAuthenticatedClient As(Models.Profile? onBehalfOf)
    {
        _profile = onBehalfOf;
        var signingKey = _profile.Keys.First(key => key.Family == SigningKey.KeyFamily.Rsa);
        _keys.SetKey(signingKey);
        return this;
    }

    public async Task<FollowState> SendFollow(Uri inbox)
    {
        var actor = _profileMapper.Map<AsAp.Actor>(_profile);
        var follow = new AsAp.Activity()
        {
            Type = "Follow",
        };
        follow.Actor.Add(actor);
        var request = new HttpRequestMessage(HttpMethod.Post, inbox)
        {
            Content = JsonContent.Create(follow, options: JsonOptions.ActivityPub),
        };
        request.Headers.Date = DateTimeOffset.Now;

        var response = await _httpClient.SendAsync(request);
        
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
        
            if (responseActivity.Type.Equals("PendingAccept", StringComparison.InvariantCultureIgnoreCase)
                || responseActivity.Type.Equals("PendingReject", StringComparison.InvariantCultureIgnoreCase))
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
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
    }
}