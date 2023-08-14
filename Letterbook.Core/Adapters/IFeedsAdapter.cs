using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IFeedsAdapter
{
    public void AddToTimeline<T>(T subject, Audience audience, Profile? boostedBy = default) where T : IContentRef;

    public void AddToTimeline<T>(T subject, ICollection<Audience> audience, Profile? boostedBy = default)
        where T : IContentRef;

    public void AddNotification<T>(Profile recipient, T subject, IEnumerable<Profile> actors, ActivityType activity)
        where T : IContentRef;

    public void RemoveFromTimelines<T>(T subject) where T : IContentRef;
    public IEnumerable<Notification> GetAggregateNotifications(Profile recipient, DateTime begin, int limit);

    public IEnumerable<Notification> GetFilteredNotifications(Profile recipient, DateTime begin,
        ActivityType typeFilter, int limit);

    public IEnumerable<Note> GetTimelineEntries(ICollection<Audience> audiences, DateTime begin, int limit,
        bool includeBoosts = true);

    public IEnumerable<IObjectRef> GetTimelineEntries(ICollection<Audience> audiences, DateTime begin, int limit,
        ICollection<string> types, bool includeBoosts = true);
}