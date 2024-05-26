using Letterbook.Adapter.TimescaleFeeds.EntityModels;
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

	public void AddToTimeline(Models.Post post, Models.Profile? sharedBy = default)
	{
		var rows = TimelinePost.Denormalize(post);
		_feedsContext.Timelines.AddRange(rows);
	}

	public async Task<int> UpdateTimeline(Models.Post post)
	{
		// leave time, id, postId, and audienceId as they are
		// this will update the content of the timelinePost, but leave it in-place in the timeline
		var tp = (TimelinePost)post;
		return await _feedsContext.Timelines
			.Where(p => p.PostId == tp.PostId)
			.ExecuteUpdateAsync(props => props
				.SetProperty(p => p.Preview, tp.Preview)
				.SetProperty(p => p.Authority, tp.Authority)
				.SetProperty(p => p.Creators, tp.Creators)
				.SetProperty(p => p.Summary, tp.Summary)
				.SetProperty(p => p.UpdatedDate, tp.UpdatedDate)
				.SetProperty(p => p.InReplyToId, tp.InReplyToId)
				.SetProperty(p => p.ThreadId, tp.ThreadId));
	}

	public void AddNotification(Models.Profile recipient, Models.Post post, Models.ActivityType activity, Models.Profile? sharedBy)
	{
		throw new NotImplementedException();
	}

	public async Task<int> RemoveFromAllTimelines(Models.Post post)
	{
		return await _feedsContext.Timelines.Where(p => p.PostId == post.Id).ExecuteDeleteAsync();
	}

	public async Task<int> RemoveFromTimelines(Models.Post post, IEnumerable<Models.Audience> removed)
	{
		var removedIds = removed.Select(a => a.FediId);
		return await _feedsContext.Timelines
			.Where(p => p.PostId == post.Id && removedIds.Contains(p.AudienceId))
			.ExecuteDeleteAsync();
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

	public IQueryable<Models.Post> GetTimelineEntries(IEnumerable<Models.Audience> audiences, DateTimeOffset before,
		int limit, bool includeShared = true)
	{
		var keys = audiences.Select(a => a.FediId);
		return _feedsContext.Timelines
				.AsNoTracking()
				.OrderBy(t => t.Time)
				.Where(t => t.Time <= before)
				.Where(t => keys.Contains(t.AudienceId))
				.Where(t => includeShared || t.SharedBy != null)
				// Equivalent to DistinctBy, but EFCore won't translate that for some reason
				// See https://github.com/npgsql/efcore.pg/issues/894
				// and https://github.com/dotnet/efcore/issues/27470
				// .DistinctBy(t => t.EntityId)
				.GroupBy(t => t.PostId).Select(g => g.First())
				.Take(limit)
				.Cast<Models.Post>();
	}

	public IQueryable<Models.Post> GetTimelineEntries(ICollection<Models.Audience> audiences, DateTime before,
		int limit, ICollection<Models.ActivityObjectType> types,
		bool includeBoosts = true)
	{
		throw new NotImplementedException();
	}

	public async Task Cancel()
	{
		if (_feedsContext.Database.CurrentTransaction is not null)
		{
			await _feedsContext.Database.RollbackTransactionAsync();
			return;
		}

		await Task.CompletedTask;
	}

	public async Task Commit()
	{
		if (_feedsContext.Database.CurrentTransaction is not null)
		{
			await _feedsContext.Database.CommitTransactionAsync();
			return;
		}

		await _feedsContext.SaveChangesAsync();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_feedsContext.Dispose();
	}

	public async Task Start()
	{
		if (_feedsContext.Database.CurrentTransaction is null)
		{
			await _feedsContext.Database.BeginTransactionAsync();
		}
	}
}