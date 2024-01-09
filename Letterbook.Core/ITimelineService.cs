using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface ITimelineService
{
    public void HandleCreate(Post post);
    public void HandleBoost(Post post);
    public void HandleUpdate(Post note);
    public void HandleDelete(Post note);
    public Task<IEnumerable<TimelineEntry>> GetFeed(Guid recipientId, DateTime begin, int limit = 40);
}