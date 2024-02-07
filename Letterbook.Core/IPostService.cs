using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core;

public interface IPostService
{
    public Task<IEnumerable<Post>> LookupPost(Uuid7 id, bool withThread = true);
    public Task<IEnumerable<Post>> LookupPost(Uri id, bool withThread = true);
    public Task<IEnumerable<Post>> LookupThread(Uuid7 id);
    public Task<IEnumerable<Post>> LookupThread(Uri id);
    public Task<Post> DraftNote(Uuid7 authorId, string contentSource, Uuid7? inReplyToId = default);
    public Task<Post> Draft(Post post, Uuid7? inReplyToId = default);
    public Task<Post> Update(Post post);
    public Task Delete(Uuid7 id);
    /// <summary>
    /// Publish a draft post
    /// </summary>
    /// <param name="id"></param>
    /// <param name="localOnly"></param>
    /// <returns></returns>
    public Task<Post> Publish(Uuid7 id, bool localOnly = false);
    
    /// <summary>
    /// Handle an inbound federated post
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    public Task<Post> Receive(Post post);
    
    /// <summary>
    /// Boost, reblog, repost, etc. Share a post with a new audience
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task Share(Uuid7 id);
    public Task Like(Uuid7 id);
    public Task<Post> AddContent(Uuid7 postId, IContent content);
    public Task<Post> RemoveContent(Uuid7 postId, Uuid7 contentId);
    public Task<Post> UpdateContent(Uuid7 postId, IContent content);
}