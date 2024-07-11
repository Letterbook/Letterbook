using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.Conversion;
using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.ActivityPub;

public class Document : IActivityPubDocument
{
	private IJsonLdSerializer _serializer;

	public Document(IJsonLdSerializer serializer)
	{
		_serializer = serializer;
	}

	public string Serialize(ASType document)
	{
		return _serializer.Serialize(document);
	}

	public AcceptActivity Accept(Models.Profile actor, ASObject asObject)
	{
		var doc = new AcceptActivity();
		doc.Actor.Add(ActorLink(actor));
		doc.Object.Add(asObject);
		return doc;
	}

	public AddActivity Add(Models.Profile actor, ASObject asObject, ASObject target)
	{
		throw new NotImplementedException();
	}

	public AnnounceActivity Announce(Models.Profile actor, Models.IContentRef content)
	{
		throw new NotImplementedException();
	}

	public BlockActivity Block(Models.Profile actor, Models.Profile target)
	{
		throw new NotImplementedException();
	}

	public CreateActivity Create(Models.Profile actor, Models.IContentRef content)
	{
		throw new NotImplementedException();
	}

	public DeleteActivity Delete(Models.Profile actor, Models.IContentRef content)
	{
		throw new NotImplementedException();
	}

	public DislikeActivity Dislike(Models.Profile actor, Models.IContentRef content)
	{
		throw new NotImplementedException();
	}

	public FollowActivity Follow(Models.Profile actor, Models.Profile target)
	{
		var doc = new FollowActivity();
		doc.Actor.Add(ActorLink(actor));
		doc.Object.Add(ActorLink(target));
		return doc;
	}

	public LikeActivity Like(Models.Profile actor, Models.IContentRef content)
	{
		throw new NotImplementedException();
	}

	public RejectActivity Reject(Models.Profile actor, ASObject asObject)
	{
		var doc = new RejectActivity();
		doc.Actor.Add(ActorLink(actor));
		doc.Object.Add(asObject);
		return doc;
	}

	public RemoveActivity Remove(Models.Profile actor, ASType @object, ASType target)
	{
		throw new NotImplementedException();
	}

	public TentativeAcceptActivity TentativeAccept(Models.Profile actor, ASObject asObject)
	{
		var doc = new TentativeAcceptActivity();
		doc.Actor.Add(ActorLink(actor));
		doc.Object.Add(asObject);
		return doc;
	}

	public UndoActivity Undo(Models.Profile actor, ASType @object)
	{
		throw new NotImplementedException();
	}

	public UpdateActivity Update(Models.Profile actor, Models.IContentRef content)
	{
		throw new NotImplementedException();
	}

	private ASLink ActorLink(Models.IFederated contentRef)
	{
		return new ASLink()
		{
			HRef = contentRef.FediId
		};
	}

	public ASActivity BuildActivity(Models.ActivityType type)
	{
		return type switch
		{
			Models.ActivityType.Accept => new AcceptActivity(),
			Models.ActivityType.Add => new AddActivity(),
			Models.ActivityType.Announce => new AnnounceActivity(),
			Models.ActivityType.Arrive => new ArriveActivity(),
			Models.ActivityType.Block => new BlockActivity(),
			Models.ActivityType.Create => new CreateActivity(),
			Models.ActivityType.Delete => new DeleteActivity(),
			Models.ActivityType.Dislike => new DislikeActivity(),
			Models.ActivityType.Flag => new FlagActivity(),
			Models.ActivityType.Follow => new FollowActivity(),
			Models.ActivityType.Ignore => new IgnoreActivity(),
			Models.ActivityType.Invite => new InviteActivity(),
			Models.ActivityType.Join => new JoinActivity(),
			Models.ActivityType.Leave => new LeaveActivity(),
			Models.ActivityType.Like => new LikeActivity(),
			Models.ActivityType.Listen => new ListenActivity(),
			Models.ActivityType.Move => new MoveActivity(),
			Models.ActivityType.Offer => new OfferActivity(),
			Models.ActivityType.Question => new QuestionActivity(),
			Models.ActivityType.Reject => new RejectActivity(),
			Models.ActivityType.Read => new ReadActivity(),
			Models.ActivityType.Remove => new RemoveActivity(),
			Models.ActivityType.TentativeReject => new TentativeRejectActivity(),
			Models.ActivityType.TentativeAccept => new TentativeAcceptActivity(),
			Models.ActivityType.Travel => new TravelActivity(),
			Models.ActivityType.Undo => new UndoActivity(),
			Models.ActivityType.Update => new UpdateActivity(),
			Models.ActivityType.View => new ViewActivity(),
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}
}