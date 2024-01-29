using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityPubDocument
{
    AcceptActivity Accept(Profile actor, ASObject asObject);
    AddActivity Add(Profile actor, ASObject asObject, ASObject target);
    AnnounceActivity Announce(Profile actor, Post content);
    BlockActivity Block(Profile actor, Profile target);
    CreateActivity Create(Profile actor, Post content);
    DeleteActivity Delete(Profile actor, Post content);
    DislikeActivity Dislike(Profile actor, Post content);
    FollowActivity Follow(Profile actor, Profile target);
    LikeActivity Like(Profile actor, Post content);
    RejectActivity Reject(Profile actor, ASObject asObject);
    RemoveActivity Remove(Profile actor, ASType @object, ASType target);
    TentativeAcceptActivity TentativeAccept(Profile actor, ASObject asObject);
    UndoActivity Undo(Profile actor, ASType @object);
    UpdateActivity Update(Profile actor, Post content);
    ASActivity BuildActivity(Models.ActivityType type);
}