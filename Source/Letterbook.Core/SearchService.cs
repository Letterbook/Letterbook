using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public partial class SearchService : ISearchService, ISearchServiceAuth
{
	private readonly ILogger<SearchService> _logger;
	private readonly IEnumerable<ISearchProvider> _providers;
	private readonly IAuthorizationService _authz;
	private readonly IDataAdapter _data;
	private IEnumerable<Claim> _claims = default!;
	private readonly CoreOptions _opts;

	public SearchService(ILogger<SearchService> logger, IEnumerable<ISearchProvider> providers, IAuthorizationService authz, IDataAdapter data, IOptions<CoreOptions> opts)
	{
		_logger = logger;
		_providers = providers;
		_authz = authz;
		_data = data;
		_opts = opts.Value;
	}

	public ISearchServiceAuth As(IEnumerable<Claim> claims)
	{
		_claims = claims;
		return this;
	}

	public async IAsyncEnumerable<IFederated> SearchAll(string query, [EnumeratorCancellation] CancellationToken cancel, int limit = 100)
	{
		var matches = LocalHandleRegex().Matches(query);
		if (matches.Count != 0)
		{
			var found = false;
			await foreach (var p in _data.AllProfiles().Where(p => p.Handle == matches[0].Value && p.Authority == _opts.BaseUri().GetAuthority()).ToAsyncEnumerable().WithCancellation(cancel))
			{
				found = true;
				yield return p;
			}
			if(found)
				yield break;
		}

		// @handle@host - search local db profiles and providers, exit early
		if (UriExtensions.TryParseHandle(query, out var handle, out var host))
		{
			var found = false;
			await foreach (var p in _data.AllProfiles().Where(p => p.Handle == handle && p.Authority == host.GetAuthority()).ToAsyncEnumerable().WithCancellation(cancel))
			{
				found = true;
				yield return p;
			}
			if (found)
				yield break;
		}

		// uri://something - search local db profiles, posts, and providers, exit early
		if (Uri.TryCreate(query, UriKind.Absolute, out var queryUri))
		{
			var found = false;
			await foreach (var p in _data.Profiles(queryUri).ToAsyncEnumerable().WithCancellation(cancel))
			{
				found = true;
				yield return p;
			}
			if (!found)
			{
				await foreach (var p in _data.Posts(queryUri).ToAsyncEnumerable().WithCancellation(cancel))
				{
					found = true;
					yield return p;
				}
			}

			if (found)
				yield break;
		}

		var pendingData = false;
		// anything else - search providers, then add to db
		foreach (var provider in _providers)
		{
			var result = await provider.SearchAny(query, cancel);
			foreach (var resource in result)
			{

				limit--;
				switch (resource)
				{
					case Post post:
						pendingData = true;
						_logger.LogDebug("Adding post {Id} to local data from search", post.FediId);
						_data.Add(post);
						break;
					case Profile profile:
						pendingData = true;
						_logger.LogDebug("Adding profile {Id} to local data from search", profile.FediId);
						_data.Add(profile);
						break;
					default:
						_logger.LogDebug("Search returned type {Type} {Id}", resource.GetType().ToString(), resource.FediId);
						continue;
				}
				if (_authz.View(_claims, resource))
					yield return resource;
			}

			if (limit <= 0) break;
		}

		if (pendingData)
			await _data.Commit();
	}

	public async IAsyncEnumerable<Profile> SearchProfiles(string query, [EnumeratorCancellation] CancellationToken cancel, int limit = 100)
	{
		foreach (var provider in _providers)
		{
			var result = await provider.SearchProfiles(query, cancel, limit);
			foreach (var profile in result)
			{
				limit--;
				if (_authz.View(_claims, profile))
					yield return profile;
			}

			if (limit <= 0) yield break;
		}
	}

	public async IAsyncEnumerable<Post> SearchPosts(string query, [EnumeratorCancellation] CancellationToken cancel, int limit = 100)
	{
		foreach (var provider in _providers)
		{
			var result = await provider.SearchPosts(query, cancel, limit);
			foreach (var profile in result)
			{
				limit--;
				if (_authz.View(_claims, profile))
					yield return profile;
			}

			if (limit <= 0) yield break;
		}
	}

	// match any string that begins with '@' and includes one or more characters other than '@'
	// I'm sure other software disagrees, but there should be no reason we can't support literally any unicode character as part of a
	// profile handle, except the special character @ which separates the username from the hostname
	[GeneratedRegex("""(?<=^@)[^@]+$""")]
	// [GeneratedRegex("""(?<=^@)\w+$""")]
	private static partial Regex LocalHandleRegex();
}