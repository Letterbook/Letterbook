namespace Letterbook.Core.Models;

public class FollowerRelation : IObjectRef
{
    public Guid Id { get; set; }
    public Profile Subject { get; set; }
    public Profile Follows { get; set; }
    public FollowerRelationState State { get; set; }
    public DateTime Date { get; set; }

    private FollowerRelation()
    {
        Id = Guid.Empty;
        Subject = default!;
        Follows = default!;
        State = default;
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