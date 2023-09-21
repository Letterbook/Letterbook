using Letterbook.Core.Values;

namespace Letterbook.Core.Models;

public class FollowerRelation : IObjectRef
{
    public Guid Id { get; set; }
    public Profile Subject { get; set; }
    public Profile Follows { get; set; }
    public FollowState State { get; set; }
    public DateTime Date { get; set; }

    private FollowerRelation()
    {
        Id = Guid.Empty;
        Subject = default!;
        Follows = default!;
        State = default;
        Date = DateTime.UtcNow;
    }

    public FollowerRelation(Profile subject, Profile follows, FollowState state)
    {
        Id = Guid.NewGuid();
        Subject = subject;
        Follows = follows;
        State = state;
        Date = DateTime.UtcNow;
    }

    Uri IObjectRef.Id
    {
        get => Subject.Id;
        set => Subject.Id = value;
    }
    Guid? IObjectRef.LocalId
    {
        get => Subject.LocalId;
        set => Subject.LocalId = value;
    }
    string IObjectRef.Authority => Subject.Authority;
}