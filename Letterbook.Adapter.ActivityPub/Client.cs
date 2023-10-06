using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
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
    private const string ProfileErr = "Activities must be performed by a specific Profile";
    private JsonSerializerOptions _jsonOpts;

    public Client(ILogger<Client> logger, HttpClient httpClient)
    {
        const string mime = """
                            application/ld+json; profile="https://www.w3.org/ns/activitystreams"
                            """;
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd(mime);
        // TODO: User-agent
        // $"dotnet/{Environment.Version.Major}.{Environment.Version.Minor} letterbook/{app version??} (baseUrl)"
        _jsonOpts = Letterbook.ActivityPub.JsonOptions.ActivityPub;
    }
    
    public IActivityPubAuthenticatedClient As(Models.Profile? onBehalfOf)
    {
        _profile = onBehalfOf;
        return this;
    }

    public async Task<FollowState> SendFollow(Uri inbox, FollowerRelation request)
    {
        if (_profile == null) throw ClientException.ProfileRequired(ProfileErr);
        var actor = _profileMapper.Map<AsAp.Actor>(_profile);
        var follow = new AsAp.Activity()
        {
            Type = "Follow",
        };
        follow.Actor.Add(actor);
        var payload = JsonContent.Create(follow, options: _jsonOpts);

        var response = await _httpClient.PostAsync(inbox, payload);
        
        var content = await ReadResponse(response);
        var responseActivity = await JsonSerializer.DeserializeAsync<AsAp.Activity>(content);
        throw new NotImplementedException();
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
    
    private async Task<Stream> ReadResponse(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.OK)
        {
            // TODO: failure handling
        }
        return await response.Content.ReadAsStreamAsync();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}