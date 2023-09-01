using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IFeedsAdapter : IDisposable
{
    public Task<int> AddToTimeline<T>(T subject, Audience audience, Profile? boostedBy = default) where T : IContentRef;

    public Task<int> AddToTimeline<T>(T subject, ICollection<Audience> audience, Profile? boostedBy = default)
        where T : IContentRef;

    public void AddNotification<T>(Profile recipient, T subject, IEnumerable<Profile> actors, ActivityType activity)
        where T : IContentRef;

    public Task<int> RemoveFromTimelines<T>(T subject) where T : IContentRef;
    public Task<int> RemoveFromTimelines<T>(T subject, ICollection<Audience> audiences) where T : IContentRef;
    public IEnumerable<Notification> GetAggregateNotifications(Profile recipient, DateTime begin, int limit);

    public IEnumerable<Notification> GetFilteredNotifications(Profile recipient, DateTime begin,
        ActivityType typeFilter, int limit);

    public IQueryable<TimelineEntry> GetTimelineEntries(ICollection<Audience> audiences, DateTime before, int limit,
        bool includeBoosts = true);

    public IQueryable<TimelineEntry> GetTimelineEntries(ICollection<Audience> audiences, DateTime before, int limit,
        ICollection<ActivityObjectType> types, bool includeBoosts = true);

    public Task Cancel();
    public Task Commit();
}