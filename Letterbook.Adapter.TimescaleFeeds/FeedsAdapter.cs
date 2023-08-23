using Letterbook.Core.Adapters;
using Models = Letterbook.Core.Models;

namespace Letterbook.Adapter.TimescaleFeeds;

public class FeedsAdapter : IFeedsAdapter
{
    public void AddToTimeline<T>(T subject, Models.Audience audience, Models.Profile? boostedBy = default) where T : Models.IContentRef
    {
        throw new NotImplementedException();
    }

    public void AddToTimeline<T>(T subject, ICollection<Models.Audience> audience, Models.Profile? boostedBy = default) where T : Models.IContentRef
    {
        throw new NotImplementedException();
    }

    public void AddNotification<T>(Models.Profile recipient, T subject, IEnumerable<Models.Profile> actors, Models.ActivityType activity) where T : Models.IContentRef
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

    public IEnumerable<Models.Notification> GetAggregateNotifications(Models.Profile recipient, DateTime begin, int limit)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Models.Notification> GetFilteredNotifications(Models.Profile recipient, DateTime begin, Models.ActivityType typeFilter, int limit)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Models.Note> GetTimelineEntries(ICollection<Models.Audience> audiences, DateTime begin, int limit, bool includeBoosts = true)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Models.IObjectRef> GetTimelineEntries(ICollection<Models.Audience> audiences, DateTime begin, int limit, ICollection<string> types,
        bool includeBoosts = true)
    {
        throw new NotImplementedException();
    }
}
