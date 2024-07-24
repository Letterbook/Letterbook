using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion;
using AutoMapper;
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
	private IFederatedActor? _actor = default;

	private static readonly IMapper DefaultMapper = new Mapper(AstMapper.Default);

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
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
	{
		if (await ValidateResponseHeaders(response, name, path, line))
			return await response.Content.ReadAsStreamAsync();
		return default;
	}

	// ValueTask is more efficient than Task when you expect to frequently just return a synchronous value
	[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
	private async ValueTask<bool> ValidateResponseHeaders(HttpResponseMessage response,
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
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
				_logger.LogDebug("Client error response had headers {@Headers} and body {Body}", response.Headers, body);
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

	public IActivityPubAuthenticatedClient As(Models.IFederatedActor? onBehalfOf)
	{
		if (onBehalfOf == null) return this;

		_actor = onBehalfOf;

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
		return SignedRequest(method, uri, _actor?.Keys, _actor?.FediId);
	}

	private HttpRequestMessage SignedRequest(HttpMethod method, Uri uri, IEnumerable<SigningKey>? keys, Uri? actorId)
	{
		var message = new HttpRequestMessage(method, uri);
		if (keys is null || actorId is null) return message;

		var httpRequestOptionsKey = new HttpRequestOptionsKey<IEnumerable<SigningKey>>(IClientSigner.SigningKeysOptionsId);
		var httpRequestOptionsProfile = new HttpRequestOptionsKey<Uri>(IClientSigner.ProfileOptionsId);
		message.Options.Set(httpRequestOptionsKey, keys);
		message.Options.Set(httpRequestOptionsProfile, actorId);

		return message;
	}


	public async Task<ClientResponse<FollowState>> SendFollow(Uri inbox, Models.Profile target)
	{
		if (_actor is not Models.Profile profileActor)
		{
			// Not sure if this is the right thing to do. Can instances follow other actors?
			throw new ClientException($"Following requires a Profile as an actor, but current actor is {_actor}");
		}

		var doc = _document.Follow(profileActor, target);
		doc.Id = $"{_actor!.FediId}#follow/{target.FediId}";
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

	public async Task<T> Fetch<T>(Uri id) where T : IFederated => await SendFetch<T>(id, SignedRequest(HttpMethod.Get, id));
	public async Task<T> Fetch<T>(Uri id, SigningKey actorSigningKey) where T : IFederated
		=> await SendFetch<T>(id, SignedRequest(HttpMethod.Get, id, new []{ actorSigningKey }, actorSigningKey.FediId));

	private async Task<T> SendFetch<T>(Uri id, HttpRequestMessage httpRequestMessage) where T : IFederated
	{
		var response = await _httpClient.SendAsync(httpRequestMessage);
		var stream = await ReadResponse(response);
		if (stream is null) throw ClientException.RemoteObjectError(id, "Peer provided no response");

		var ast = await _jsonLdSerializer.DeserializeAsync<ASType>(stream);
		var mapped = DefaultMapper.Map<T>(ast);
		if (mapped.FediId == id)
		{
			return mapped;
		}

		_logger.LogError("Remote fetch for Object {Id} returned {ObjectId}", id, mapped.FediId);
		_logger.LogDebug("Tried to map {ASType} to {ModelType}", ast?.Type, typeof(T));
		throw ClientException.RemoteObjectError(mapped.FediId, "Peer provided object is not the same as requested");
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_httpClient.Dispose();
	}
}