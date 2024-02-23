using Letterbook.Core.Values;
using Medo;

namespace Letterbook.Core.Models;

public class FollowerRelation
{
    private Uuid7 _id = Uuid7.NewUuid7();

    public Guid Id
    {
        get => _id.ToGuid();
        set => Uuid7.FromGuid(value);
    }

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
    
    public Uuid7 GetId() => _id;
    public string GetId25() => _id.ToId25String();
}