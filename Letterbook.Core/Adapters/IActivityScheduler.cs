using ActivityPub.Types.AS;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Deliver an ActivityPub message to its recipient out of band from the current context.
/// Messages are placed in a work queue for subsequent processing.
/// This allows ActivityPub Http requests (and responses) to be processed outside the current Asp action, for example.
/// This also enables messages to easily be retried on certain kinds of delivery failure.
/// </summary>
public interface IActivityScheduler
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
	/// Deliver a Post that's been published.
	/// </summary>
	/// <remarks>The Post will be wrapped in a Create activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="post"></param>
	/// <param name="onBehalfOf"></param>
	/// <param name="extraMention"></param>
	/// <returns></returns>
	public Task Publish(Uri inbox, Post post, Profile onBehalfOf, Mention? extraMention = default);

	/// <summary>
	/// Deliver a Post that's been updated.
	/// </summary>
	/// <remarks>The Post will be wrapped in an Update activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="post"></param>
	/// <param name="onBehalfOf"></param>
	/// <param name="extraMention"></param>
	/// <returns></returns>
	public Task Update(Uri inbox, Post post, Profile onBehalfOf, Mention? extraMention = default);

	/// <summary>
	/// Deliver a Post that's been deleted.
	/// </summary>
	/// <remarks>The Post will be sent as an ID only, in a Delete activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="post"></param>
	/// <param name="onBehalfOf"></param>
	/// <returns></returns>
	public Task Delete(Uri inbox, Post post, Profile onBehalfOf);

	/// <summary>
	/// Share a previously published Post.
	/// </summary>
	/// <remarks>The Post will be sent as an ID only, in an Announce activity</remarks>
	/// <param name="inbox"></param>
	/// <param name="post"></param>
	/// <param name="onBehalfOf"></param>
	/// <returns></returns>
	public Task Share(Uri inbox, Post post, Profile onBehalfOf);

	/// <summary>
	/// Send a Like message regarding a Post.
	/// </summary>
	/// <param name="inbox"></param>
	/// <param name="post"></param>
	/// <param name="onBehalfOf"></param>
	/// <returns></returns>
	public Task Like(Uri inbox, Post post, Profile onBehalfOf);

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

	/// <summary>
	/// Construct an AP Flag document concerning the report's subjects and send it to the specified inbox. Reports are
	/// always sent using a special system actor.
	/// </summary>
	/// <param name="inbox"></param>
	/// <param name="report"></param>
	/// <returns></returns>
	public Task Report(Uri inbox, ModerationReport report);
}