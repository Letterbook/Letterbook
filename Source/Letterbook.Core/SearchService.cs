using System.Runtime.CompilerServices;
using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public class SearchService : ISearchService, ISearchServiceAuth
{
	private readonly IEnumerable<ISearchProvider> _providers;
	private readonly IAuthorizationService _authz;
	private IEnumerable<Claim> _claims = default!;

	public SearchService(IEnumerable<ISearchProvider> providers, IAuthorizationService authz)
	{
		_providers = providers;
		_authz = authz;
	}

	public ISearchServiceAuth As(IEnumerable<Claim> claims)
	{
		_claims = claims;
		return this;
	}

	public async IAsyncEnumerable<IFederated> SearchAll(string query, [EnumeratorCancellation] CancellationToken cancel, int limit = 100)
	{
		foreach (var provider in _providers)
		{
			var result = await provider.SearchAny(query, cancel);
			foreach (var profile in result)
			{
				limit--;
				if (_authz.View(_claims, profile))
					yield return profile;
			}

			if (limit <= 0) yield break;
		}
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
}