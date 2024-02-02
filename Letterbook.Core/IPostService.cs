using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core;

public interface IPostService
{
    public Task<Post?> LookupPost(Uuid7 id);
    public Task<Post> DraftNote(Uuid7 authorId, string contentSource, Uuid7? inReplyToId = default);
    public Task<Post> Draft(Post post, Uuid7? inReplyToId = default);
    public Task<Post> Update(Post post);
    public Task Delete(Uuid7 id);
    public Task<Post> Publish(Uuid7 id, bool localOnly = false);
    public Task Share(Uuid7 id);
    public Task Like(Uuid7 id);
    public Task<Post> AddContent(Uuid7 postId, IContent content);
    public Task<Post> RemoveContent(Uuid7 postId, Uuid7 contentId);
    public Task<Post> UpdateContent(Uuid7 postId, IContent content);
}