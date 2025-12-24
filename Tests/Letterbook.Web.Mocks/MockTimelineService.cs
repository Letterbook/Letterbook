using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Medo;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Mocks;

public class MockTimelineService : ITimelineService, IAuthzTimelineService
{
	private readonly ILogger<MockTimelineService> _log;
	private TimelineService _timelineService;
	private FakePost _posts;
	private FakeProfile _profiles;

	public MockTimelineService(ILogger<MockTimelineService> log, IOptions<CoreOptions> opts, TimelineService timelineService)
	{
		_log = log;
		_timelineService = timelineService;
		_profiles = new FakeProfile();
		_posts = new FakePost(_profiles);
	}

	public Task HandlePublish(Models.Post post)
	{
		return _timelineService.HandlePublish(post);
	}
	public Task HandleShare(Models.Post post, Models.Profile sharedBy)
	{
		return _timelineService.HandleShare(post, sharedBy);
	}
	public Task HandleUpdate(Models.Post post, Models.Post oldPost)
	{
		return _timelineService.HandleUpdate(post, oldPost);
	}
	public Task HandleDelete(Models.Post note)
	{
		return _timelineService.HandleDelete(note);
	}
	public Task<IEnumerable<Models.Post>> GetFeed(Models.ProfileId profileId, DateTimeOffset begin, int limit = 40)
	{
		_log.LogInformation("Called the mock service");
		var result = Enumerable.Range(0, limit)
			.Select(_ => new FakePost(_profiles.Generate()).Generate());
		return Task.FromResult(result);
		// return _timelineService.GetFeed(profileId, begin, limit);
	}

	public IAuthzTimelineService As(IEnumerable<Claim> claims)
	{
		_timelineService.As(claims);
		return this;
	}
}