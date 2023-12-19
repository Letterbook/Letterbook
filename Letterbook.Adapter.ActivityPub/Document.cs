using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.ActivityPub;

public class Document : IActivityPubDocument
{
    public AcceptActivity Accept(Models.Profile actor, ASType @object)
    {
        throw new NotImplementedException();
    }

    public AddActivity Add(Models.Profile actor, ASType @object, ASType target)
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
        throw new NotImplementedException();
    }

    public LikeActivity Like(Models.Profile actor, Models.IContentRef content)
    {
        throw new NotImplementedException();
    }

    public RejectActivity Reject(Models.Profile actor, ASType @object)
    {
        throw new NotImplementedException();
    }

    public RemoveActivity Remove(Models.Profile actor, ASType @object, ASType target)
    {
        throw new NotImplementedException();
    }

    public TentativeAcceptActivity Pending(Models.Profile actor, ASType @object)
    {
        throw new NotImplementedException();
    }

    public UndoActivity Undo(Models.Profile actor, ASType @object)
    {
        throw new NotImplementedException();
    }

    public UpdateActivity Update(Models.Profile actor, Models.IContentRef content)
    {
        throw new NotImplementedException();
    }
}