using System.Runtime.CompilerServices;
using System.Text;
using Letterbook.Adapter.TimescaleFeeds.Extensions;
using Letterbook.Core.Adapters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models = Letterbook.Core.Models;

namespace Letterbook.Adapter.TimescaleFeeds;

public class FeedsAdapter : IFeedsAdapter
{
    private readonly FeedsContext _feedsContext;
    private IDbContextTransaction _transaction;

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
            EntityId = subject.Id.ToString(),
            AudienceKey = audience.Id.ToString(),
            AudienceName = null,
            CreatedBy = subject.Creators.Select(c => c.Id.ToString()).ToArray(),
            Authority = subject.Authority,
            BoostedBy = boostedBy?.Id.ToString(),
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

    public void AddNotification<T>(Models.Profile recipient, T subject, IEnumerable<Models.Profile> actors,
        Models.ActivityType activity) where T : Models.IContentRef
    {
        throw new NotImplementedException();
    }

    public async Task<int> RemoveFromTimelines<T>(T subject) where T : Models.IContentRef
    {
        Start();
        return await _feedsContext.Database.ExecuteSqlAsync(
            $"""
            DELETE FROM "Feeds"
            WHERE "EntityId" = {subject.Id.ToString()};
            """);
    }

    public async Task<int> RemoveFromTimelines<T>(T subject, ICollection<Models.Audience> audiences) where T : Models.IContentRef
    {
        Start();
        var keys = audiences.Select(a => $"{a.Id}" as object);
        var builder = new StringBuilder(
            """
            DELETE FROM "Feeds"
            WHERE "EntityId" = {0}
            AND "AudienceKey" in (
            """);
        builder.AppendJoin(',', Enumerable.Range(1, audiences.Count).Select(i => $"{{{i}}}"));
        builder.Append(");");
        var sql = FormattableStringFactory.Create(builder.ToString(), keys.Prepend(subject.Id.ToString()).ToArray());
        
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
        var keys = audiences.Select(a => a.Id.ToString());
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