using System.Runtime.CompilerServices;
using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core;

public class SearchService : ISearchService, ISearchServiceAuth
{
	private readonly IEnumerable<IGlobalSearchProvider> _all;
	private readonly IEnumerable<IPostSearchProvider> _posts;
	private readonly IEnumerable<IProfileSearchProvider> _profiles;
	private readonly IAuthorizationService _authz;
	private IEnumerable<Claim> _claims = default!;

	public SearchService(IEnumerable<IGlobalSearchProvider> all, IEnumerable<IPostSearchProvider> posts,
		IEnumerable<IProfileSearchProvider> profiles, IAuthorizationService authz)
	{
		_all = all;
		_posts = posts;
		_profiles = profiles;
		_authz = authz;
	}

	public ISearchServiceAuth As(IEnumerable<Claim> claims)
	{
		_claims = claims;
		return this;
	}

	public async IAsyncEnumerable<IFederated> SearchAll(string query, [EnumeratorCancellation] CancellationToken cancel, int limit = 100)
	{
		foreach (var provider in _all)
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
		foreach (var provider in _profiles)
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

	public IAsyncEnumerable<Post> SearchPosts()
	{
		throw new NotImplementedException();
	}
}