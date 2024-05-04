using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IPostEvents : IEventChannel
{
	/// <summary>
	/// the post was created
	/// </summary>
	/// <param name="post"></param>
	public void Created(Post post);

	/// <summary>
	/// The Post was deleted
	/// </summary>
	/// <param name="post"></param>
	public void Deleted(Post post);

	/// <summary>
	/// The post was updated
	/// </summary>
	/// <param name="post"></param>
	public void Updated(Post post);

	/// <summary>
	/// The post was published
	/// </summary>
	/// <param name="post"></param>
	public void Published(Post post);

	/// <summary>
	/// The Post was received for a Profile
	/// </summary>
	/// <param name="post"></param>
	/// <param name="recipient">The Profile who received the Post</param>
	public void Received(Post post, Profile recipient);

	/// <summary>
	/// The Post was Liked by a Profile
	/// </summary>
	/// <param name="post"></param>
	/// <param name="likedBy">The Profile who liked the Post</param>
	public void Liked(Post post, Profile likedBy);

	/// <summary>
	/// The Post was shared by a Profile
	/// </summary>
	/// <param name="post"></param>
	/// <param name="sharedBy">The Profile who shared the Post</param>
	public void Shared(Post post, Profile sharedBy);

	/// <summary>
	/// The Type of the emitted event's Data field
	/// </summary>
	public class Data
	{
		/// <summary>
		/// The Post that was the subject of the event
		/// </summary>
		public required Post Post { get; init; }

		/// <summary>
		/// The Profile that was the object of the event
		/// </summary>
		public string? ProfileId { get; init; }
	}
}