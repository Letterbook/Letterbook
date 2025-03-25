using System.Security.Claims;
using Letterbook.Core;
using Medo;

namespace Letterbook.Web.Mocks;

public class MockTimelineService : ITimelineService, IAuthzTimelineService
{
	private readonly ILogger<MockTimelineService> _log;
	private TimelineService _timelineService;


	public MockTimelineService(ILogger<MockTimelineService> log, TimelineService timelineService)
	{
		_log = log;
		_timelineService = timelineService;
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
	public Task<IEnumerable<Models.Post>> GetFeed(Uuid7 profileId, DateTimeOffset begin, int limit = 40)
	{
		_log.LogInformation("Called the mock service");
		return _timelineService.GetFeed(profileId, begin, limit);
	}

	public IAuthzTimelineService As(IEnumerable<Claim> claims)
	{
		_timelineService.As(claims);
		return this;
	}
}