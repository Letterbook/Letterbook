using System.Text.Json.Nodes;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityPubDocument
{
	public string Serialize(ASType document);
	public string Serialize(JsonNode node);
	public JsonNode? SerializeToNode(ASType document);
	AcceptActivity Accept(Profile actor, ASObject asObject);
	AddActivity Add(Profile actor, ASObject asObject, ASObject target);
	AnnounceActivity Announce(Profile actor, Uri content);
	BlockActivity Block(Profile actor, Profile target);
	CreateActivity Create(Profile actor, ASObject createdObject);
	DeleteActivity Delete(Profile actor, Uri content);
	DislikeActivity Dislike(Profile actor, IContentRef content);
	FlagActivity Flag(Profile systemActor, Uri inbox, ModerationReport report, ModerationReport.Scope scope, Profile? subject = null);
	FollowActivity Follow(Profile actor, Profile target, Uri? id = null, bool implicitId = false);
	LikeActivity Like(Profile actor, Uri content);
	RejectActivity Reject(Profile actor, ASObject asObject);
	RemoveActivity Remove(Profile actor, ASType @object, ASType target);
	TentativeAcceptActivity TentativeAccept(Profile actor, ASObject asObject);
	UndoActivity Undo(Profile actor, ASObject @object, Uri? id = null, bool implicitId = false);
	UpdateActivity Update(Profile actor, ASObject content);
	ASObject FromPost(Post post);
	ASLink ObjectId(IFederated linkable);
	ASActivity BuildActivity(ActivityType type);
}