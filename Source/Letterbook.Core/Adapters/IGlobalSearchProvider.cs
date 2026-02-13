using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IGlobalSearchProvider
{
	public Task<IEnumerable<IFederated>> SearchAny(string query, CancellationToken cancellationToken, int limit = 100);
}