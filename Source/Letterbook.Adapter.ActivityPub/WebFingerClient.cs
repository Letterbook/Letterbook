using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DarkLink.Web.WebFinger.Client;
using DarkLink.Web.WebFinger.Shared;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.ActivityPub;

public partial class WebFingerClient : ISearchProvider
{
	private readonly ILogger<WebFingerClient> _logger;
	private readonly HttpClient _httpClient;
	private readonly IActivityPubClient _apClient;

	[GeneratedRegex("^acct:(?=[^/])")]
	private static partial Regex MatchAcct();

	public WebFingerClient(ILogger<WebFingerClient> logger, HttpClient httpClient, IActivityPubClient apClient)
	{
		_logger = logger;
		_httpClient = httpClient;
		_apClient = apClient;
	}

	public async Task<IEnumerable<Models.IFederated>> SearchAny(string query, CancellationToken cancellationToken, int limit = 1) =>
		await SearchProfiles(query, cancellationToken);

	public async Task<IEnumerable<Models.Profile>> SearchProfiles(string query, CancellationToken cancellationToken, int limit = 1)
	{
		query = MatchAcct().Replace(query, "");
		query = string.Join('@', query.Split('@', 2,
			StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

		UriBuilder builder;
		try
		{
			builder = new UriBuilder(query);
		}
		catch (UriFormatException )
		{
			_logger.LogDebug("Can't process search for {Query}", query);
			return [];
		}

		if (string.IsNullOrEmpty(builder.UserName) ||
		    string.IsNullOrEmpty(builder.Host) ||
		    builder.Path != "/" ||
		    !string.IsNullOrEmpty(builder.Password) ||
		    !string.IsNullOrEmpty(builder.Query) ||
		    !builder.Uri.IsDefaultPort ||
		    builder.Uri.IsLoopback ||
		    builder.Uri.HostNameType != UriHostNameType.Dns)
		{
			_logger.LogDebug("Can't process search for {Query}", query);
			return [];
		}

		var acct = $"{builder.UserName}@{builder.Host}";
		builder.Fragment = "";
		builder.Scheme = Uri.UriSchemeHttps;
		builder.Port = 443;
		builder.UserName = "";
		var host = builder.Uri;

		var webfinger = await _httpClient.WebfingerQuery(host, acct, cancellationToken);
		if (webfinger == null)
		{
			_logger.LogDebug("Invalid response to query {Host} for {Resource}", host, acct);
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

public static async Task<JsonResourceDescriptor?> WebfingerQuery(this HttpClient client, Uri host, string acct, CancellationToken cancellationToken)
	{
		var uri = new UriBuilder(host)
		{
			Path = "/.well-known/webfinger",
			Query = $"resource={Uri.EscapeDataString("acct:" + acct)}"
		}.Uri;

		var options = new JsonSerializerOptions();
		options.Converters.Add(new JsonResourceDescriptorConverter());
		return await client.GetFromJsonAsync<JsonResourceDescriptor>(uri, options, cancellationToken);
	}
}