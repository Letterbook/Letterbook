using Letterbook.Core;
using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.ActivityPub;

public class FallbackSearchProvider(ISearchProvider primary, ISearchProvider? secondary) : ISearchProvider
{
	public Task<IEnumerable<Models.IFederated>> SearchAny(string query, CancellationToken cancellationToken, CoreOptions options, int limit = 100)
		=> primary.SearchAny(query, cancellationToken, options, limit);

	public async Task<IEnumerable<Models.Profile>> SearchProfiles(string query, CancellationToken cancellationToken, CoreOptions options, int limit = 100)
	{
		ArgumentNullException.ThrowIfNull(primary);

		var primaryResult = (await primary.SearchProfiles(query, cancellationToken, options, limit)).ToList();

		if (primaryResult.Any())
			return primaryResult;

		if (secondary != null)
			return await secondary.SearchProfiles(query, cancellationToken, options, limit);

		return primaryResult;
	}

	public Task<IEnumerable<Models.Post>> SearchPosts(string query, CancellationToken cancellationToken, CoreOptions options, int limit = 100)
	{
		throw new NotImplementedException("Leaving this to see what happens");
	}
}