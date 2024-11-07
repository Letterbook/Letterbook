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
	public Task<Post?> LookupPost(Uuid7 id, bool withThread = true);
	public Task<Post?> LookupPost(Uri id, bool withThread = true);
	public Task<ThreadContext?> LookupThread(Uuid7 id);
	public Task<ThreadContext?> LookupThread(Uri id);
	public Task<Post> DraftNote(ProfileId authorId, string contentSource, Uuid7? inReplyToId = default);
	public Task<Post> Draft(ProfileId _profileId, Post post, Uuid7? inReplyToId = default, bool publish = false);
	public Task<Post> Update(Uuid7 postId, Post post);
	public Task Delete(Uuid7 id);
	/// <summary>
	/// Boost, reblog, repost, etc. Share a post with a new audience
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Task Share(Uuid7 id);
	public Task Like(Uuid7 id);
	public Task<Post> AddContent(Uuid7 postId, Content content);
	public Task<Post> RemoveContent(Uuid7 postId, Uuid7 contentId);
	public Task<Post> UpdateContent(Uuid7 postId, Uuid7 contentId, Content content);

	/// <summary>
	/// Publish a draft post
	/// </summary>
	/// <param name="id"></param>
	/// <param name="localOnly"></param>
	/// <returns></returns>
	public Task<Post> Publish(Uuid7 id, bool localOnly = false);

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