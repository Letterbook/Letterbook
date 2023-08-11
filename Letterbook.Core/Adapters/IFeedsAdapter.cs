using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IFeedsAdapter
{
    public void AddToTimeline(Note note, Audience audience, bool boosted = false);
    public void AddToTimeline(Image image, Audience audience, bool boosted = false);
    public void AddNotification(Profile recipient, Note subject, Profile actor, ActivityType activity);
    public IEnumerable<Notification> GetAggregateNotifications(Profile recipient, DateTime begin, int limit = 20);

    public IEnumerable<Notification> GetFilteredNotifications(Profile recipient, DateTime begin,
        ActivityType typeFilter, int limit = 20);

    public IEnumerable<Note> GetTimelineEntries(ICollection<Audience> audiences, DateTime begin,
        bool includeBoosts = true, int limit = 20);

    public IEnumerable<IObjectRef> GetTimelineEntries(ICollection<Audience> audiences, DateTime begin,
        ICollection<string> types, bool includeBoosts = true, int limit = 20);
}