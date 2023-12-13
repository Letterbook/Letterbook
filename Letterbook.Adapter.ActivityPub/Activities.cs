using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;

namespace Letterbook.Adapter.ActivityPub;

public static class Activities
{
    public static ASActivity BuildActivity(Models.ActivityType type)
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

    public static ASActivity BuildActivity(Models.ActivityType type, Models.Profile actor)
    {
        var activity = BuildActivity(type);
        activity.Actor.Add(actor.Id);

        return activity;
    }

    public static ASActivity BuildActivity(Models.ActivityType type, Models.Profile actor, ASObject @object)
    {
        var activity = BuildActivity(type, actor);
        activity.Object.Add(@object);

        return activity;
    }
}