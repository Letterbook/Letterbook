using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.ActivityPub;

public class Document : IActivityPubDocument
{
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

    private ASLink ActorLink(Models.IObjectRef contentRef)
    {
        return new ASLink()
        {
            HRef = contentRef.Id
        };
    }
}