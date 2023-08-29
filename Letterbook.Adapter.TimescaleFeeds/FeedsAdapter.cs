using System.Runtime.CompilerServices;
using System.Text;
using Letterbook.Adapter.TimescaleFeeds.Entities;
using Letterbook.Adapter.TimescaleFeeds.Extensions;
using Letterbook.Core.Adapters;
using Microsoft.EntityFrameworkCore;
using Models = Letterbook.Core.Models;

namespace Letterbook.Adapter.TimescaleFeeds;

public class FeedsAdapter : IFeedsAdapter
{
    private readonly FeedsContext _feedsContext;
    private bool _canceled;

    public FeedsAdapter(FeedsContext feedsContext)
    {
        _feedsContext = feedsContext;
        _canceled = false;
    }

    public void AddToTimeline<T>(T subject, Models.Audience audience, Models.Profile? boostedBy = default)
        where T : Models.IContentRef
    {
        var line = new Entry()
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

        _feedsContext.Feeds.FromSql(
            $"""
             INSERT INTO "Feeds" ("Time", "Type", "EntityId", "AudienceKey", "AudienceName", "CreatedBy", "Authority", "BoostedBy", "CreatedDate")
             VALUES ({line.Time}, {line.Type}, {line.EntityId}, {line.AudienceKey}, {null}, ARRAY [{string.Join(',', line.CreatedBy)}], {line.Authority}, {line.BoostedBy}, {line.CreatedDate})
             """);
    }

    public void AddToTimeline<T>(T subject, ICollection<Models.Audience> audience, Models.Profile? boostedBy = default)
        where T : Models.IContentRef
    {
        if(!audience.Any()) return;
        foreach (var each in audience)
        {
            AddToTimeline(subject, each, boostedBy);
        }
        
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
        _feedsContext.Feeds.FromSql(FormattableStringFactory.Create(builder.ToString(), values));
    }

    public void AddNotification<T>(Models.Profile recipient, T subject, IEnumerable<Models.Profile> actors,
        Models.ActivityType activity) where T : Models.IContentRef
    {
        throw new NotImplementedException();
    }

    public void RemoveFromTimelines<T>(T subject) where T : Models.IContentRef
    {
        throw new NotImplementedException();
    }

    public void RemoveFromTimelines<T>(T subject, ICollection<Models.Audience> audiences) where T : Models.IContentRef
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

    public IEnumerable<Models.Note> GetTimelineEntries(ICollection<Models.Audience> audiences, DateTime begin,
        int limit, bool includeBoosts = true)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Models.IObjectRef> GetTimelineEntries(ICollection<Models.Audience> audiences, DateTime begin,
        int limit, ICollection<string> types,
        bool includeBoosts = true)
    {
        throw new NotImplementedException();
    }

    public void Cancel() => _canceled = true;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if(!_canceled) _feedsContext.SaveChanges();
        _feedsContext.Dispose();
    }
}