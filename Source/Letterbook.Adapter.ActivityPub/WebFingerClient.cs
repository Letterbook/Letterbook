using System.Collections.Immutable;
using DarkLink.Web.WebFinger.Client;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.ActivityPub;

public class WebFingerClient : IGlobalSearchProvider, IProfileSearchProvider
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

	public async Task<IEnumerable<Models.IFederated>> SearchAny(string query, CancellationToken cancellationToken) =>
		await SearchProfiles(query, cancellationToken);

	public async Task<IEnumerable<Models.Profile>> SearchProfiles(string query, CancellationToken cancellationToken)
	{
		var parts = query.Split('@', 2,
			StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length != 2)
		{
			_logger.LogDebug("Can't process search for {Query}", query);
			return [];
		}

		if (!Uri.TryCreate($"https://{parts[1]}", UriKind.Absolute, out var host) ||
		    !Uri.TryCreate($"acct:{parts[0]}", UriKind.Absolute, out var resource))
		{
			_logger.LogDebug("Can't parse {Query}", query);
			return [];
		}

		var webfinger = await _httpClient.GetResourceDescriptorAsync(host, resource, cancellationToken);
		if (webfinger == null)
		{
			_logger.LogDebug("Invalid response to query {Host} for {Resource}", host, resource);
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

	private record ValueLink(string Type, Uri Href, string Rel, ImmutableDictionary<Uri, string?> Properties, ImmutableDictionary<string, string> Titles)
    	{
    		public override string ToString()
    		{
    			return $"{{ Type = {Type}, Href = {Href}, Rel = {Rel}, Properties = {Properties}, Titles = {Titles} }}";
    		}
    	}

}