using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityPubDocument
{
	public string Serialize(ASType document);
	AcceptActivity Accept(Profile actor, ASObject asObject);
	AddActivity Add(Profile actor, ASObject asObject, ASObject target);
	AnnounceActivity Announce(Profile actor, IContentRef content);
	BlockActivity Block(Profile actor, Profile target);
	CreateActivity Create(Profile actor, IContentRef content);
	DeleteActivity Delete(Profile actor, IContentRef content);
	DislikeActivity Dislike(Profile actor, IContentRef content);
	FollowActivity Follow(Profile actor, Profile target);
	LikeActivity Like(Profile actor, IContentRef content);
	RejectActivity Reject(Profile actor, ASObject asObject);
	RemoveActivity Remove(Profile actor, ASType @object, ASType target);
	TentativeAcceptActivity TentativeAccept(Profile actor, ASObject asObject);
	UndoActivity Undo(Profile actor, ASObject @object);
	UpdateActivity Update(Profile actor, IContentRef content);
	ASActivity BuildActivity(Models.ActivityType type);
}