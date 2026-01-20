using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IPostSearchProvider
{
	public Task<IEnumerable<Post>> SearchPost(string query, CancellationToken cancellationToken);
}