using System.Security.Claims;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core;

public interface IPostService
{
	public IAuthzPostService As(IEnumerable<Claim> claims, Uuid7 profileId);
}

public interface IAuthzPostService
{
	public Task<Post?> LookupPost(PostId id, bool withThread = true);
	public Task<Post?> LookupPost(Uri id, bool withThread = true);
	public Task<ThreadContext?> LookupThread(Uuid7 id);
	public Task<ThreadContext?> LookupThread(Uri id);
	public Task<Post> DraftNote(ProfileId authorId, string contentSource, PostId? inReplyToId = default);
	public Task<Post> Draft(ProfileId _profileId, Post post, PostId? inReplyToId = default, bool publish = false);
	public Task<Post> Update(PostId postId, Post post);
	public Task Delete(PostId id);
	/// <summary>
	/// Boost, reblog, repost, etc. Share a post with a new audience
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Task Share(Uuid7 id);
	public Task Like(Uuid7 id);
	public Task<Post> AddContent(PostId postId, Content content);
	public Task<Post> RemoveContent(PostId postId, Uuid7 contentId);
	public Task<Post> UpdateContent(PostId postId, Uuid7 contentId, Content content);

	/// <summary>
	/// Publish a draft post
	/// </summary>
	/// <param name="id"></param>
	/// <param name="localOnly"></param>
	/// <returns></returns>
	public Task<Post> Publish(PostId id, bool localOnly = false);

	/// <summary>
	/// Handle an inbound Create activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public Task<Post> ReceiveCreate(Post post);

	/// <summary>
	/// Handle an inbound Update activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public Task<Post> ReceiveUpdate(Post post);

	/// <summary>
	/// Handle an inbound Update activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public Task<Post> ReceiveUpdate(Uri post);

	/// <summary>
	/// Handle an inbound Delete activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public Task<Post> ReceiveDelete(Uri post);

	/// <summary>
	/// Handle an inbound Announce activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <param name="announcedBy"></param>
	/// <returns></returns>
	public Task<Post> ReceiveAnnounce(Post post, Uri announcedBy);

	/// <summary>
	/// Handle an inbound Announce activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <param name="announcedBy"></param>
	/// <returns></returns>
	public Task<Post> ReceiveAnnounce(Uri post, Uri announcedBy);

	/// <summary>
	/// Handle an inbound Undo activity on a previously announced post
	/// </summary>
	/// <param name="post"></param>
	/// <param name="likedBy"></param>
	/// <returns></returns>
	public Task<Post> ReceiveUndoAnnounce(Uri post, Uri likedBy);

	/// <summary>
	/// Handle an inbound Like activity on a previously announced post
	/// </summary>
	/// <param name="post"></param>
	/// <param name="likedBy"></param>
	/// <returns></returns>
	public Task<Post> ReceiveLike(Uri post, Uri likedBy);

	/// <summary>
	/// Handle an inbound Undo activity on a previously liked post
	/// </summary>
	/// <param name="post"></param>
	/// <param name="likedBy"></param>
	/// <returns></returns>
	public Task<Post> ReceiveUndoLike(Uri post, Uri likedBy);
}