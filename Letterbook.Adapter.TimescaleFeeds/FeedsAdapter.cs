using Letterbook.Core.Adapters;
using Microsoft.EntityFrameworkCore;
using Models = Letterbook.Core.Models;

namespace Letterbook.Adapter.TimescaleFeeds;

public class FeedsAdapter : IFeedsAdapter
{
	private readonly FeedsContext _feedsContext;

	public FeedsAdapter(FeedsContext feedsContext)
	{
		_feedsContext = feedsContext;
	}

	public Task<int> AddToTimeline(Models.Post post, Models.Profile? sharedBy = default)
	{
		throw new NotImplementedException();
	}

	public void AddNotification(Models.Profile recipient, Models.Post post, Models.ActivityType activity, Models.Profile? sharedBy)
	{
		throw new NotImplementedException();
	}

	public Task<int> RemoveFromTimelines(Models.Post post)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Models.Notification> GetAggregateNotifications(Models.Profile recipient, DateTime begin,
		int limit)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Models.Notification> GetFilteredNotifications(Models.Profile recipient, DateTime begin,
		Models.ActivityType typeFilter, int limit)
	{
		throw new NotImplementedException();
	}

	public IQueryable<Models.TimelineEntry> GetTimelineEntries(ICollection<Models.Audience> audiences, DateTime before,
		int limit, bool includeBoosts = true)
	{
		var keys = audiences.Select(a => a.FediId.ToString());
		return _feedsContext.Feeds
			.AsNoTracking()
			.Where(t => t.Time <= before)
			.Where(t => keys.Contains(t.AudienceKey))
			.Where(t => includeBoosts || t.BoostedBy != null)
			.GroupBy(t => t.EntityId).Select(g => g.First())
			// Equivalent to DistinctBy, but EFCore won't translate that for some reason
			// .DistinctBy(t => t.EntityId)
			.Take(limit);
	}

	public IQueryable<Models.TimelineEntry> GetTimelineEntries(ICollection<Models.Audience> audiences, DateTime before,
		int limit, ICollection<Models.ActivityObjectType> types,
		bool includeBoosts = true)
	{
		throw new NotImplementedException();
	}

	public Task Cancel()
	{
		if (_feedsContext.Database.CurrentTransaction is not null)
		{
			return _feedsContext.Database.RollbackTransactionAsync();
		}

		return Task.CompletedTask;
	}
	public Task Commit()
	{
		if (_feedsContext.Database.CurrentTransaction is not null)
		{
			return _feedsContext.Database.CommitTransactionAsync();
		}
		return _feedsContext.SaveChangesAsync();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_feedsContext.Dispose();
	}

	private void Start()
	{
		if (_feedsContext.Database.CurrentTransaction is null)
		{
			_feedsContext.Database.BeginTransaction();
		}
	}
}