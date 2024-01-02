using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IActivityEventService
{
    void Created(Post post);
    void Updated(Post post);
    void Deleted(Post post);
    void Flagged(Post post);
    void Liked(Post post);
    void Boosted(Post post);
    void Approved(Post post);
    void Rejected(Post post);
    void Requested(Post post);
    void Offered(Post post);
    void Mentioned(Post post);
}