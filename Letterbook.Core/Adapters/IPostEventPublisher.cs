using System.Security.Claims;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Events and a corresponding channel related to <see cref="Post">Posts</see>
/// </summary>
public interface IPostEventPublisher
{
	/// <summary>
	/// the post was created
	/// </summary>
	/// <param name="post"></param>
	/// <param name="createdBy"></param>
	/// <param name="claims"></param>
	public Task Created(Post post, Uuid7 createdBy, IEnumerable<Claim> claims);

	/// <summary>
	/// The Post was deleted
	/// </summary>
	/// <param name="post"></param>
	/// <param name="deletedBy"></param>
	/// <param name="claims"></param>
	public Task Deleted(Post post, Uuid7 deletedBy, IEnumerable<Claim> claims);

	/// <summary>
	/// The post was updated
	/// </summary>
	/// <param name="post"></param>
	/// <param name="updatedBy"></param>
	/// <param name="claims"></param>
	public Task Updated(Post post, Uuid7 updatedBy, IEnumerable<Claim> claims);

	/// <summary>
	/// The post was published
	/// </summary>
	/// <param name="post"></param>
	/// <param name="publishedBy"></param>
	/// <param name="claims"></param>
	public Task Published(Post post, Uuid7 publishedBy, IEnumerable<Claim> claims);

	/// <summary>
	/// The Post was Liked by a Profile
	/// </summary>
	/// <param name="post"></param>
	/// <param name="likedBy">The Profile who liked the Post</param>
	/// <param name="claims"></param>
	public Task Liked(Post post, Uuid7 likedBy, IEnumerable<Claim> claims);

	/// <summary>
	/// The Post was shared by a Profile
	/// </summary>
	/// <param name="post"></param>
	/// <param name="sharedBy">The Profile who shared the Post</param>
	/// <param name="claims"></param>
	public Task Shared(Post post, Uuid7 sharedBy, IEnumerable<Claim> claims);
}