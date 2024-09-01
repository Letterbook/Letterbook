using System.Security.Claims;
using Letterbook.Core.Events;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Events and a corresponding channel related to <see cref="Post">Posts</see>
/// </summary>
public interface IPostEventPublisher : IEventChannel
{
	/// <summary>
	/// the post was created
	/// </summary>
	/// <param name="post"></param>
	/// <param name="claims"></param>
	public Task Created(Post post, IEnumerable<Claim> claims);

	/// <summary>
	/// The Post was deleted
	/// </summary>
	/// <param name="post"></param>
	/// <param name="claims"></param>
	public Task Deleted(Post post, IEnumerable<Claim> claims);

	/// <summary>
	/// The post was updated
	/// </summary>
	/// <param name="post"></param>
	/// <param name="claims"></param>
	public Task Updated(Post post, IEnumerable<Claim> claims);

	/// <summary>
	/// The post was published
	/// </summary>
	/// <param name="post"></param>
	/// <param name="claims"></param>
	public Task Published(Post post, IEnumerable<Claim> claims);

	/// <summary>
	/// The Post was Liked by a Profile
	/// </summary>
	/// <param name="post"></param>
	/// <param name="likedBy">The Profile who liked the Post</param>
	/// <param name="claims"></param>
	public Task Liked(Post post, Profile likedBy, IEnumerable<Claim> claims);

	/// <summary>
	/// The Post was shared by a Profile
	/// </summary>
	/// <param name="post"></param>
	/// <param name="sharedBy">The Profile who shared the Post</param>
	/// <param name="claims"></param>
	public Task Shared(Post post, Profile sharedBy, IEnumerable<Claim> claims);
}