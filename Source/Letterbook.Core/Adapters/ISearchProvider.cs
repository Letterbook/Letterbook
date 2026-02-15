using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface ISearchProvider
{
	public Task<IEnumerable<IFederated>> SearchAny(string query, CancellationToken cancellationToken, CoreOptions options, int limit = 100);
	public Task<IEnumerable<Profile>> SearchProfiles(string query, CancellationToken cancellationToken, CoreOptions options,
		int limit = 100)
		=> Task.FromResult(Enumerable.Empty<Profile>());
	public Task<IEnumerable<Post>> SearchPosts(string query, CancellationToken cancellationToken, CoreOptions options, int limit = 100)
		=> Task.FromResult(Enumerable.Empty<Post>());
}