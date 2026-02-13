using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IProfileSearchProvider
{
	public Task<IEnumerable<Profile>> SearchProfiles(string query, CancellationToken cancellationToken, int limit = 100);
}