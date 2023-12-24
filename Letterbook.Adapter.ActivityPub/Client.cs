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
    private readonly IActivityPubDocument _document;
    private readonly JsonLdSerializerOptions _jsonLdSerializerOptions;
    private Models.Profile? _profile = default;
    
    private static readonly IMapper ProfileMapper = new Mapper(ProfileMappers.DefaultProfile);
    private static readonly IMapper AsApMapper = new Mapper(Mappers.AsApMapper.Config);
    private static readonly IMapper ToLinkMapper = new Mapper(ProfileMappers.DefaultLink);

    public Client(ILogger<Client> logger, HttpClient httpClient, IJsonLdSerializer jsonLdSerializer, IActivityPubDocument document)
    {
        _logger = logger;
        _httpClient = httpClient;
        _jsonLdSerializer = jsonLdSerializer;
        _document = document;
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

    // ValueTask is more efficient than Task when you expect to frequently just return a synchronous value
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
                _logger.LogInformation("Received client error from {Method} {Uri}", response.RequestMessage?.Method, response.RequestMessage?.RequestUri);
                _logger.LogDebug("Client error response had headers {Headers} and body {Body}", response.Headers, body);
                throw ClientException.RequestError(response.StatusCode,
                    $"Couldn't {response.RequestMessage?.Method.ToString() ?? "METHOD UNKNOWN"} AP resource ({response.RequestMessage?.RequestUri})",
                    body, name: name, path: path, line: line);
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

    private async Task<HttpResponseMessage> Send(Uri inbox, ASType document)
    {
        _logger.LogDebug("Sending document to {Inbox}", inbox);
        var message = SignedRequest(HttpMethod.Post, inbox, document);
        var response = await _httpClient.SendAsync(message);
        
        await ValidateResponseHeaders(response);
        _logger.LogDebug("Sent document to {Inbox} - {Result}", inbox, response.StatusCode);
        return response;
    }
    
    public IActivityPubAuthenticatedClient As(Models.Profile? onBehalfOf)
    {
        if (onBehalfOf == null) return this;
        
        _profile = onBehalfOf;
        
        return this;
    }

    private HttpRequestMessage SignedRequest(HttpMethod method, Uri uri, ASType document)
    {
        var message = SignedRequest(method, uri);
        var payload = _jsonLdSerializer.Serialize(document);
        message.Content = new StringContent(payload, Constants.LdJsonHeader);
        _logger.LogDebug("Sending {Activity}", payload);

        return message;
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

    public async Task<ClientResponse<FollowState>> SendFollow(Uri inbox, Models.Profile target)
    {
        var doc = _document.Follow(_profile!, target);
        doc.Id = $"{_profile!.Id}#follow/{target.Id}";
        var response = await Send(inbox, doc);
        
        // To my knowledge, there are no existing fedi services that can actually respond in-band,
        // so we'll just assume Pending for the moment
        // TODO(FEP): Research and/or publish reply negotiation mechanisms
        return new ClientResponse<FollowState>()
        {
            StatusCode = response.StatusCode,
            Data = FollowState.Pending,
            DeliveredAddress = response.RequestMessage?.RequestUri
        };
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
        ASActivity acceptObject = Activities.BuildActivity(activityToAccept);
        var accept = Activities.BuildActivity(ActivityType.Accept, _profile, acceptObject);

        acceptObject.Actor.Add(requestorId);
        acceptObject.Object.Add(_profile.Id);
        
        return SendAccept(inbox, accept);
    }

    private async Task SendAccept(Uri inbox, ASActivity activity)
    {
        var message = SignedRequest(HttpMethod.Post, inbox, activity);
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

    public async Task<ClientResponse<object>> SendDocument(Uri inbox, ASType document)
    {
        _logger.LogDebug("Sending document to {Inbox}", inbox);
        var message = SignedRequest(HttpMethod.Post, inbox, document);
        var response = await _httpClient.SendAsync(message);
        
        await ValidateResponseHeaders(response);
        _logger.LogDebug("Sent document to {Inbox} - {Result}", inbox, response.StatusCode);
        return new ClientResponse<object>()
        {
            StatusCode = response.StatusCode,
            Data = default,
            DeliveredAddress = response.RequestMessage?.RequestUri
        };
    }

    public async Task<ClientResponse<object>> SendDocument(Uri inbox, string document)
    {
        _logger.LogDebug("Sending document to {Inbox}", inbox);
        var message = SignedRequest(HttpMethod.Post, inbox);
        message.Content = new StringContent(document, Constants.LdJsonHeader);
        var response = await _httpClient.SendAsync(message);
        
        await ValidateResponseHeaders(response);
        _logger.LogDebug("Sent document to {Inbox} - {Result}", inbox, response.StatusCode);
        return new ClientResponse<object>()
        {
            StatusCode = response.StatusCode,
            Data = default,
            DeliveredAddress = response.RequestMessage?.RequestUri
        };
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