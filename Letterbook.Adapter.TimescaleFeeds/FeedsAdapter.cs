using System.Runtime.CompilerServices;
using System.Text;
using Letterbook.Adapter.TimescaleFeeds.Extensions;
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
        // _canceled = false;
    }
    
    public async Task<int> AddToTimeline<T>(T subject, Models.Audience audience, Models.Profile? boostedBy = default)
        where T : Models.IContentRef
    {
        Start();
        var line = new Models.TimelineEntry()
        {
            Type = subject.Type,
            EntityId = subject.FediId.ToString(),
            AudienceKey = audience.FediId.ToString(),
            AudienceName = null,
            CreatedBy = subject.Creators.Select(c => c.FediId.ToString()).ToArray(),
            Authority = subject.Authority,
            BoostedBy = boostedBy?.FediId.ToString(),
            CreatedDate = subject.CreatedDate
        };

        return await _feedsContext.Database.ExecuteSqlAsync(
            $"""
             INSERT INTO "Feeds" ("Time", "Type", "EntityId", "AudienceKey", "AudienceName", "CreatedBy", "Authority", "BoostedBy", "CreatedDate")
             VALUES ({line.Time}, {line.Type}, {line.EntityId}, {line.AudienceKey}, {null}, ARRAY [{string.Join(',', line.CreatedBy)}], {line.Authority}, {line.BoostedBy}, {line.CreatedDate});
             """);
    }

    public async Task<int> AddToTimeline<T>(T subject, ICollection<Models.Audience> audience,
        Models.Profile? boostedBy = default)
        where T : Models.IContentRef
    {
        if(!audience.Any()) return 0;
        
        Start();
        // language=NONE suppress jetbrains embedded sql highlighting because it's more annoying than helpful here
        var builder = new StringBuilder(
            """
            INSERT INTO "Feeds" ("Time", "Type", "EntityId", "AudienceKey", "AudienceName", "CreatedBy", "Authority", "BoostedBy", "CreatedDate")
            VALUES 
            """);

        for (var i = 0; i < audience.Count; i++)
        {
            builder.AppendEntryRow(i);
            if (i + 1 < audience.Count) builder.Append(",\n");
        }

        var values = new List<object?>();
        foreach (var each in audience)
        {
            values.AddRange(subject.ToEntryValues(each, boostedBy));
        }

        builder.Append(';');
        var sql = FormattableStringFactory.Create(builder.ToString(), values.ToArray());
        return await _feedsContext.Database.ExecuteSqlAsync(sql);
    }

    public Task<int> AddToTimeline(Models.Post post, Models.Profile? sharedBy = default)
    {
        throw new NotImplementedException();
    }

    public void AddNotification<T>(Models.Profile recipient, T subject, IEnumerable<Models.Profile> actors,
        Models.ActivityType activity) where T : Models.IContentRef
    {
        throw new NotImplementedException();
    }

    public void AddNotification(Models.Profile recipient, Models.Post post, Models.ActivityType activity, Models.Profile? sharedBy)
    {
        throw new NotImplementedException();
    }

    public async Task<int> RemoveFromTimelines<T>(T subject) where T : Models.IContentRef
    {
        Start();
        return await _feedsContext.Database.ExecuteSqlAsync(
            $"""
            DELETE FROM "Feeds"
            WHERE "EntityId" = {subject.FediId.ToString()};
            """);
    }

    public Task<int> RemoveFromTimelines(Models.Post post)
    {
        throw new NotImplementedException();
    }

    public async Task<int> RemoveFromTimelines<T>(T subject, ICollection<Models.Audience> audiences) where T : Models.IContentRef
    {
        Start();
        var keys = audiences.Select(a => $"{a.FediId}" as object);
        var builder = new StringBuilder(
            """
            DELETE FROM "Feeds"
            WHERE "EntityId" = {0}
            AND "AudienceKey" in (
            """);
        builder.AppendJoin(',', Enumerable.Range(1, audiences.Count).Select(i => $"{{{i}}}"));
        builder.Append(");");
        var sql = FormattableStringFactory.Create(builder.ToString(), keys.Prepend(subject.FediId.ToString()).ToArray());
        
        return await _feedsContext.Database.ExecuteSqlAsync(sql);
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

        return Task.CompletedTask;
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