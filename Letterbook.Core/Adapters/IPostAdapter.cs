using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

#pragma warning disable CS1998
public interface IPostAdapter
{
    public Task<Post?> LookupPost(Uuid7 postId);
    public void Add(Post post);
    public void AddRange(IEnumerable<Post> posts);
    
    public Task Cancel();
    public Task Commit();
}