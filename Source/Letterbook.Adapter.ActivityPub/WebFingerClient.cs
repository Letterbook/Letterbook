using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using DarkLink.Web.WebFinger.Shared;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.ActivityPub;

public class WebFingerClient : ISearchProvider
{
	private readonly ILogger<WebFingerClient> _logger;
	private readonly HttpClient _httpClient;
	private readonly IActivityPubClient _apClient;

	public WebFingerClient(ILogger<WebFingerClient> logger, HttpClient httpClient, IActivityPubClient apClient)
	{
		_logger = logger;
		_httpClient = httpClient;
		_apClient = apClient;
	}

	public async Task<IEnumerable<Models.IFederated>> SearchAny(string query, CancellationToken cancellationToken, CoreOptions options,
		int limit = 100) =>
		await SearchProfiles(query, cancellationToken, options);

	public async Task<IEnumerable<Models.Profile>> SearchProfiles(string query, CancellationToken cancellationToken, CoreOptions options,
		int limit = 100)
	{
		if (!UriExtensions.TryParseHandle(query, out var handle, out var host))
		{
			_logger.LogDebug("Can't process search for {Query}", query);
			return [];
		}

		var webfinger = await WebfingerQuery(cancellationToken, host, handle);

		if (webfinger == null)
		{
			_logger.LogDebug("Invalid response to query {Host} for {Resource}", host, handle);
			return [];
		}

		var selfLinks = webfinger.Links
			.Where(l => l.Relation.Equals("self", StringComparison.InvariantCultureIgnoreCase))
			.Where(l => l is { Type: not null, Href: not null })
			.Select(l => new ValueLink(l.Type!, l.Href!, l.Relation, l.Properties, l.Titles))
			.ToLookup(l => l.Type);

		var uris = selfLinks["application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\""]
			.Concat(selfLinks["application/activity+json"])
			.Concat(selfLinks["application/ld+json"])
			.Select(l => l.Href)
			.Distinct();

		var links = new List<Models.Profile>();
		foreach (var link in uris)
		{
			try
			{
				var result = await _apClient.Fetch<Models.Profile>(link, cancellationToken);
				links.Add(result);
			}
			catch (ClientException) { }
			catch (OperationCanceledException)
			{
				if (links.Count > 0)
					return links;
				throw;
			}
		}

		return links;
	}

	private async Task<JsonResourceDescriptor?> WebfingerQuery(CancellationToken cancellationToken, Uri host, string handle)
	{
		try
		{
			return await _httpClient.WebfingerQuery(host, handle, cancellationToken);
		}
		catch (HttpRequestException e)
		{
			_logger.LogWarning("Failed to query '{handle}' at '{host}'. {e}", handle, host, e);
			return null;
		}
	}

	private record ValueLink(
		string Type,
		Uri Href,
		string Rel,
		ImmutableDictionary<Uri, string?> Properties,
		ImmutableDictionary<string, string> Titles)
	{
		public override string ToString()
		{
			return $"{{ Type = {Type}, Href = {Href}, Rel = {Rel}, Properties = {Properties}, Titles = {Titles} }}";
		}
	}
}

public static class WebfingerExtensions {
	public static async Task<JsonResourceDescriptor?> WebfingerQuery(this HttpClient client, Uri host, string handle, CancellationToken cancellationToken)
	{
		var uri = new UriBuilder(host)
		{
			Path = "/.well-known/webfinger",
			Query = $"resource={Uri.EscapeDataString($"acct:{handle}@{host.Host}")}"
		}.Uri;

		var options = new JsonSerializerOptions();
		options.Converters.Add(new JsonResourceDescriptorConverter());
		return await client.GetFromJsonAsync<JsonResourceDescriptor>(uri, options, cancellationToken);
	}
}