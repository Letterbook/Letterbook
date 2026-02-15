using System.Runtime.CompilerServices;
using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class SearchService : ISearchService, ISearchServiceAuth
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
		// first provider is local data
		// if we have the data, just use that
		var data = _providers.First();
		var found = false;
		foreach (var r in await data.SearchAny(query, cancel, _opts, limit))
		{
			found = true;
			limit--;
			yield return r;
		}

		if (found)
			yield break;

		var pendingData = false;
		// for all other providers, add the discovered result to local data before returning
		foreach (var provider in _providers.Skip(1))
		{
			var result = await provider.SearchAny(query, cancel, _opts);
			foreach (var resource in result)
			{

				limit--;
				switch (resource)
				{
					case Post post:
						pendingData = true;
						_logger.LogDebug("Discovered post {Id} from search", resource.FediId);
						_data.Add(post);
						break;
					case Profile profile:
						pendingData = true;
						_logger.LogDebug("Discovered profile {Id} from search", resource.FediId);
						_data.Add(profile);
						break;
					default:
						_logger.LogDebug("Search returned unexpected type {Type} {Id}", resource.GetType().ToString(), resource.FediId);
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
		// first provider is local data
		// if we have the data, just use that
		var data = _providers.First();
		var found = false;
		foreach (var r in await data.SearchProfiles(query, cancel, _opts, limit))
		{
			found = true;
			limit--;
			yield return r;
		}

		if (found)
			yield break;

		var pendingData = false;
		// for all other providers, add the discovered result to local data before returning
		foreach (var provider in _providers.Skip(1))
		{
			var result = await provider.SearchProfiles(query, cancel, _opts);
			foreach (var resource in result)
			{

				limit--;
				pendingData = true;
				_logger.LogDebug("Discovered profile {Id} from search", resource.FediId);
				_data.Add(resource);

				if (_authz.View(_claims, resource))
					yield return resource;
			}

			if (limit <= 0) break;
		}

		if (pendingData)
			await _data.Commit();
	}

	public async IAsyncEnumerable<Post> SearchPosts(string query, [EnumeratorCancellation] CancellationToken cancel, int limit = 100)
	{
		// first provider is local data
		// if we have the data, just use that
		var data = _providers.First();
		var found = false;
		foreach (var r in await data.SearchPosts(query, cancel, _opts, limit))
		{
			found = true;
			limit--;
			yield return r;
		}

		if (found)
			yield break;

		var pendingData = false;
		// for all other providers, add the discovered result to local data before returning
		foreach (var provider in _providers.Skip(1))
		{
			var result = await provider.SearchPosts(query, cancel, _opts);
			foreach (var resource in result)
			{

				limit--;
				pendingData = true;
				_logger.LogDebug("Discovered post {Id} from search", resource.FediId);
				_data.Add(resource);

				if (_authz.View(_claims, resource))
					yield return resource;
			}

			if (limit <= 0) break;
		}

		if (pendingData)
			await _data.Commit();
	}
}