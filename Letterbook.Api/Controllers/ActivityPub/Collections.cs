using Letterbook.Api.Attributes;

namespace Letterbook.Api.Controllers.ActivityPub;

public enum Collections
{
    [StringValue("{id}/[action]")]
    Inbox,
    [StringValue("{id}/[action]")]
    Outbox,
    [StringValue("[action]")]
    SharedInbox,
    [StringValue("{id}/collections/[action]")]
    Liked,
    [StringValue("{id}/collections/[action]")]
    Following,
    [StringValue("{id}/collections/[action]")]
    Followers
}