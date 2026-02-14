using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface ISearchService
{
	public ISearchServiceAuth As(IEnumerable<Claim> claims);
}

public interface ISearchServiceAuth
{
	public IAsyncEnumerable<IFederated> SearchAll(string query, CancellationToken cancel, int limit = 100);
	public IAsyncEnumerable<Profile> SearchProfiles(string query, CancellationToken cancel, int limit = 100);
	public IAsyncEnumerable<Post> SearchPosts(string query, CancellationToken cancel, int limit = 100);
}