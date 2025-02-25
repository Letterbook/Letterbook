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
	FollowActivity Follow(Profile actor, Profile target, Uri? id = null, bool implicitId = false);
	FlagActivity Flag(Profile systemActor, Uri inbox, ModerationReport report, bool fullContext = false);
	FlagActivity Flag(Profile systemActor, Uri inbox, ModerationReport report, Profile subject, bool fullContext = false);
	LikeActivity Like(Profile actor, Uri content);
	RejectActivity Reject(Profile actor, ASObject asObject);
	RemoveActivity Remove(Profile actor, ASType @object, ASType target);
	TentativeAcceptActivity TentativeAccept(Profile actor, ASObject asObject);
	UndoActivity Undo(Profile actor, ASObject @object, Uri? id = null, bool implicitId = false);
	UpdateActivity Update(Profile actor, ASObject content);
	ASObject FromPost(Post post);
	ASLink ObjectId(IFederated linkable);
	ASActivity BuildActivity(Models.ActivityType type);
}