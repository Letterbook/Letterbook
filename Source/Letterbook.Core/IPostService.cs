using System.Security.Claims;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core;

public interface IPostService
{
	public IAuthzPostService As(IEnumerable<Claim> claims);
}

public interface IAuthzPostService
{
	public Task<Post?> LookupPost(PostId id, bool withThread = true);
	public Task<Post?> LookupPost(ProfileId asProfile, Uri id, bool withThread = true);
	public Task<ThreadContext?> LookupThread(ProfileId asProfile, ThreadId id);
	public Task<ThreadContext?> LookupThread(ProfileId asProfile, Uri id);
	public Task<Post> DraftNote(ProfileId asProfile, string contentSource, PostId? inReplyToId = default);
	public Task<Post> Draft(ProfileId asProfile, Post post, PostId? inReplyToId = default, bool publish = false);
	public Task<Post> Update(ProfileId asProfile, PostId postId, Post post);
	public Task Delete(ProfileId asProfile, PostId id);

	/// <summary>
	/// Boost, reblog, repost, etc. Share a post with a new audience
	/// </summary>
	/// <param name="asProfile"></param>
	/// <param name="id"></param>
	/// <returns></returns>
	public Task Share(ProfileId asProfile, PostId id);
	public Task Like(ProfileId asProfile, PostId id);
	public Task<Post> AddContent(ProfileId asProfile, PostId postId, Content content);
	public Task<Post> RemoveContent(ProfileId asProfile, PostId postId, Uuid7 contentId);
	public Task<Post> UpdateContent(ProfileId asProfile, PostId postId, Uuid7 contentId, Content content);

	/// <summary>
	/// Publish a draft post
	/// </summary>
	/// <param name="asProfile"></param>
	/// <param name="id"></param>
	/// <param name="localOnly"></param>
	/// <returns></returns>
	public Task<Post> Publish(ProfileId asProfile, PostId id, bool localOnly = false);

	/// <summary>
	/// Handle an inbound Create activity on a post
	/// </summary>
	/// <param name="posts"></param>
	/// <returns>The resultant Post if it could be handled in-band, otherwise null</returns>
	public Task<IEnumerable<Post>> ReceiveCreate(IEnumerable<Post> posts);

	/// <summary>
	/// Handle an inbound Update activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <returns>The resultant Post if it could be handled in-band, otherwise null</returns>
	public Task<IEnumerable<Post>> ReceiveUpdate(IEnumerable<Post> posts);

	/// <summary>
	/// Handle an inbound Update activity on a post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public Task<Post> ReceiveUpdate(Uri post);

	/// <summary>
	/// Handle an inbound Delete activity on a post
	/// </summary>
	/// <param name="items"></param>
	/// <returns></returns>
	public Task<IEnumerable<Post>> ReceiveDelete(IEnumerable<Uri> items);

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