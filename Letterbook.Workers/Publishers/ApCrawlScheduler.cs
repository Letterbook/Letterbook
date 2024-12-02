using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Workers.Contracts;
using MassTransit;

namespace Letterbook.Workers.Publishers;

public class ApCrawlScheduler : IApCrawlScheduler
{
	private readonly IBus _bus;

	public ApCrawlScheduler(IBus bus)
	{
		_bus = bus;
	}

	public Task CrawlPost(ProfileId onBehalfOf, Uri target, int depthLimit = 0) =>
		_bus.Publish(new CrawlerMessage
		{
			OnBehalfOf = onBehalfOf,
			Resource = target,
			DepthLimit = depthLimit,
			Type = CrawlerMessage.ExpectedType.Post
		});

	public Task CrawlThread(ProfileId onBehalfOf, Uri target, PostId postId, int depthLimit = 0) =>
		_bus.Publish(new CrawlerMessage
		{
			OnBehalfOf = onBehalfOf,
			Resource = target,
			DepthLimit = depthLimit,
			Type = CrawlerMessage.ExpectedType.Thread
		});

	public Task CrawlProfile(ProfileId onBehalfOf, Uri target, int depthLimit = 0) =>
		_bus.Publish(new CrawlerMessage
		{
			OnBehalfOf = onBehalfOf,
			Resource = target,
			DepthLimit = depthLimit,
			Type = CrawlerMessage.ExpectedType.Profile
		});

	public Task CrawlOutbox(ProfileId onBehalfOf, ProfileId target, int depthLimit = 0) =>
		_bus.Publish(new CrawlerMessage
		{
			OnBehalfOf = onBehalfOf,
			Profile = target,
			Resource = default,
			DepthLimit = depthLimit,
			Type = CrawlerMessage.ExpectedType.ProfileOutbox
		});

	public Task CrawlStream(ProfileId onBehalfOf, Uri target, ProfileId profileId, int depthLimit = 0) =>
		_bus.Publish(new CrawlerMessage
		{
			OnBehalfOf = onBehalfOf,
			Profile = profileId,
			Resource = target,
			DepthLimit = depthLimit,
			Type = CrawlerMessage.ExpectedType.ProfileStream
		});
}