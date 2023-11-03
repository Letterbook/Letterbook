using Letterbook.Core.Values;

namespace Letterbook.Core.Models;

public class FollowerRelation : IObjectRef
{
    public Guid Id { get; set; }
    public Profile Follower { get; set; }
    public Profile Follows { get; set; }
    public FollowState State { get; set; }
    public DateTime Date { get; set; }

    private FollowerRelation()
    {
        Id = Guid.Empty;
        Follower = default!;
        Follows = default!;
        State = default;
        Date = DateTime.UtcNow;
    }

    public FollowerRelation(Profile follower, Profile follows, FollowState state)
    {
        Id = Guid.NewGuid();
        Follower = follower;
        Follows = follows;
        State = state;
        Date = DateTime.UtcNow;
    }

    Uri IObjectRef.Id
    {
        get => Follower.Id;
        set => Follower.Id = value;
    }
    Guid? IObjectRef.LocalId
    {
        get => Follower.LocalId;
        set => Follower.LocalId = value;
    }
    string IObjectRef.Authority => Follower.Authority;
}