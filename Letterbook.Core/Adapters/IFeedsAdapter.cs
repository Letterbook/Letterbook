using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IFeedsAdapter : IDisposable
{
	/// <summary>
	/// Add a Post to the front of the timeline.
	///
	/// This will make it available to members of any audience that should include this post.
	/// Typically, that includes the followers of the author(s), or of the sharing profile, and anyone addressed or
	/// mentioned in the post
	/// </summary>
	/// <param name="post"></param>
	/// <param name="sharedBy"></param>
	/// <returns></returns>
	public Task<int> AddToTimeline(Post post, Profile? sharedBy = default);

	/// <summary>
	/// Update the existing entries for a post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public Task<int> UpdateTimeline(Post post);

	/// <summary>
	/// Add a notification of an event to the recipient's notification feed.
	///
	/// This will make the notification visible to the specified recipient.
	/// </summary>
	/// <param name="recipient"></param>
	/// <param name="post"></param>
	/// <param name="activity"></param>
	/// <param name="sharedBy"></param>
	public void AddNotification(Profile recipient, Post post, ActivityType activity, Profile? sharedBy = default);

	/// <summary>
	/// Delete a post from the timeline.
	///
	/// This will remove every instance of a post from the timeline for every audience.
	/// Typically, this would be done if the post itself has been deleted, but we could also support unpublishing.
	/// </summary>
	/// <param name="post"></param>
	/// <returns>The number of affected records</returns>
	public Task<int> RemoveFromTimelines(Post post);

	/// <summary>
	/// Query notifications for the recipient.
	///
	/// Results are pre-aggregated by subject.
	/// </summary>
	/// <param name="recipient"></param>
	/// <param name="begin"></param>
	/// <param name="limit"></param>
	/// <returns></returns>
	public IEnumerable<Notification> GetAggregateNotifications(Profile recipient, DateTime begin, int limit);

	/// <summary>
	/// Query notifications for a specific type of event.
	///
	/// Results are pre-aggregated by subject.
	/// </summary>
	/// <param name="recipient"></param>
	/// <param name="begin"></param>
	/// <param name="typeFilter"></param>
	/// <param name="limit"></param>
	/// <returns></returns>
	public IEnumerable<Notification> GetFilteredNotifications(Profile recipient, DateTime begin,
		ActivityType typeFilter, int limit);

	/// <summary>
	/// Query the timeline for the given audiences.
	///
	/// Results are pre-aggregated by post. This means posts that have been shared repeatedly will only appear once in
	/// a given query. They may appear again in successive queries.
	/// </summary>
	/// <param name="audiences"></param>
	/// <param name="before"></param>
	/// <param name="limit"></param>
	/// <param name="includeShared"></param>
	/// <returns></returns>
	public IQueryable<Post> GetTimelineEntries(IEnumerable<Audience> audiences, DateTimeOffset before, int limit,
		bool includeShared = true);

	/// <summary>
	/// Query the timeline for the given audiences, filtered by post content type.
	///
	/// For instance, get only Pictures, or only Notes. Results are pre-aggregated by post.
	/// </summary>
	/// <param name="audiences"></param>
	/// <param name="before"></param>
	/// <param name="limit"></param>
	/// <param name="types"></param>
	/// <param name="includeBoosts"></param>
	/// <returns></returns>
	public IQueryable<Post> GetTimelineEntries(ICollection<Audience> audiences, DateTime before, int limit,
		ICollection<ActivityObjectType> types, bool includeBoosts = true);

	public Task Cancel();
	public Task Commit();
}