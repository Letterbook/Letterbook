using ActivityPub.Types.AS;
using Letterbook.Core.Events;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Deliver an ActivityPub message to its recipient out of band from the current context.
/// Messages are placed in a work queue for subsequent processing.
/// This allows ActivityPub Http requests (and responses) to be processed outside the current Asp action, for example.
/// This also enables messages to easily be retried on certain kinds of delivery failure.
/// </summary>
public interface IActivityMessagePublisher : IEventChannel
{
	/// <summary>
	/// Deliver an arbitrary AP document to the inbox
	/// </summary>
	/// <param name="inbox"></param>
	/// <param name="activity"></param>
	/// <param name="onBehalfOf"></param>
	/// <returns></returns>
	public Task Deliver(Uri inbox, ASType activity, Profile? onBehalfOf);

	/// <summary>
	/// Construct an AP document representing a request to follow the target and send it to the specified inbox
	/// </summary>
	/// <remarks>This will be represented as a simple Follow activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="target"></param>
	/// <param name="actor"></param>
	/// <returns></returns>
	public Task Follow(Uri inbox, Profile target, Profile actor);

	/// <summary>
	/// Construct an AP document that signals the actor is no longer following the target and send it to the specified inbox
	/// </summary>
	/// <remarks>This will be represented as an Undo:Follow activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="target"></param>
	/// <param name="actor"></param>
	/// <returns></returns>
	public Task Unfollow(Uri inbox, Profile target, Profile actor);

	/// <summary>
	/// Construct an AP document that signals the target profile has been removed as a follower and send it to the specified inbox
	/// </summary>
	/// <remarks>This will be represented as an Undo:Accept:Follow Activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="target"></param>
	/// <param name="actor"></param>
	/// <returns></returns>
	public Task RemoveFollower(Uri inbox, Profile target, Profile actor);

	/// <summary>
	/// Construct an AP document that accepts the target's follow request and send it do the specified inbox
	/// </summary>
	/// <remarks>This will be represented as an Accept:Follow Activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="target">The profile that previously made the request</param>
	/// <param name="actor">The current actor profile that is accepting the request</param>
	/// <returns></returns>
	public Task AcceptFollower(Uri inbox, Profile target, Profile actor);

	/// <summary>
	/// Construct an AP document that rejects the target's follow request and send it do the specified inbox
	/// </summary>
	/// <remarks>This will be represented as a Reject:Follow Activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="target">The profile that previously made the request</param>
	/// <param name="actor">The current actor profile that is accepting the request</param>
	/// <returns></returns>
	public Task RejectFollower(Uri inbox, Profile target, Profile actor);

	/// <summary>
	/// Construct an AP document that notifies the target's follow request is pending acceptance and send it
	/// to the specified inbox
	/// </summary>
	/// <remarks>This will be represented as a TentativeAccept:Follow Activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="target">The profile that previously made the request</param>
	/// <param name="actor">The current actor profile that is accepting the request</param>
	/// <returns></returns>
	public Task PendingFollower(Uri inbox, Profile target, Profile actor);
}