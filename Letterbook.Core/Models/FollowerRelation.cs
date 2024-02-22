using Letterbook.Core.Values;
using Medo;

namespace Letterbook.Core.Models;

public class FollowerRelation
{
    public Uuid7 Id { get; set; } = Uuid7.NewUuid7();

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
        Follower = default!;
        Follows = default!;
        State = default;
        Date = DateTime.UtcNow;
    }

    public FollowerRelation(Profile follower, Profile follows, FollowState state)
    {
        Follower = follower;
        Follows = follows;
        State = state;
        Date = DateTime.UtcNow;
    }
}