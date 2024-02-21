using Letterbook.Core.Values;
using Medo;

namespace Letterbook.Core.Models;

public class FollowerRelation : IObjectRef
{
    public Guid Id { get; set; }
    /// <summary>
    /// This Profile is following another
    /// </summary>
    public Profile Follower { get; set; }
    /// <summary>
    /// The Profile that is being followed
    /// </summary>
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
    Uuid7 IObjectRef.LocalId
    {
        get => Follower.LocalId;
        set => Follower.LocalId = value;
    }
    string IObjectRef.Authority => Follower.Authority;
}