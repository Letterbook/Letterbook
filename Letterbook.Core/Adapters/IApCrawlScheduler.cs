using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Schedule AP objects to be crawled and processed
/// </summary>
public interface IApCrawlScheduler
{
	/// <summary>
	/// Crawl a post-like object
	/// </summary>
	/// <param name="onBehalfOf">The Profile to authenticate as on the request</param>
	/// <param name="target">The object to retrieve</param>
	/// <param name="depthLimit">The maximum depth to recursively crawl objects discovered from this object</param>
	/// <returns></returns>
	public Task CrawlPost(ProfileId onBehalfOf, Uri target, int depthLimit = 0);

	/// <summary>
	/// Crawl a thread of post-like objects attached to a known post
	/// </summary>
	/// <param name="onBehalfOf">The Profile to authenticate as on the request</param>
	/// <param name="postId"></param>
	/// <param name="target">The object to retrieve</param>
	/// <param name="depthLimit">The maximum depth to recursively crawl objects discovered from this object</param>
	/// <returns></returns>
	public Task CrawlThread(ProfileId onBehalfOf, Uri target, PostId postId, int depthLimit = 0);

	/// <summary>
	/// Crawl a Profile
	/// </summary>
	/// <param name="onBehalfOf">The Profile to authenticate as on the request</param>
	/// <param name="target">The object to retrieve</param>
	/// <param name="depthLimit">The maximum depth to recursively crawl objects discovered from this object</param>
	/// <returns></returns>
	public Task CrawlProfile(ProfileId onBehalfOf, Uri target, int depthLimit = 0);

	/// <summary>
	/// Crawl a Profile's outbox endpoint
	/// </summary>
	/// <param name="onBehalfOf">The Profile to authenticate as on the request</param>
	/// <param name="target">The Profile to retrieve</param>
	/// <param name="depthLimit">The maximum depth to recursively crawl objects discovered from this object</param>
	/// <returns></returns>
	public Task CrawlOutbox(ProfileId onBehalfOf, ProfileId target, int depthLimit = 0);

	/// <summary>
	/// Crawl a custom stream belonging to a Profile
	/// </summary>
	/// <param name="onBehalfOf">The Profile to authenticate as on the request</param>
	/// <param name="target"></param>
	/// <param name="profileId"></param>
	/// <param name="depthLimit">The maximum depth to recursively crawl objects discovered from this object</param>
	/// <returns></returns>
	public Task CrawlStream(ProfileId onBehalfOf, Uri target, ProfileId profileId, int depthLimit = 0);
}