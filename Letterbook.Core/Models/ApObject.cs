namespace Letterbook.Core.Models;

public class ApObject
{
    private ApObject()
    {
        Id = default!;
        Type = ActivityObjectType.None;
        Host = default!;
        Actor = default!;
    }

    public Uri Id { get; set; }
    public ActivityObjectType Type { get; set; }
    public string? LocalId { get; set; }
    public Uri Host { get; set; }
    public Actor Actor { get; set; }
    public List<Actor> AddressedTo { get; set; } = new();
    public List<Actor> AddressedBto { get; set; } = new();
    public List<Actor> AddressedCc { get; set; } = new();
    public List<Actor> AddressedBcc { get; set; } = new();
    public List<Audience> Audience { get; set; } = new();
    // TODO: content (languages are hard)
    // TODO: versions (ugh)
    // TODO: dates (versions make this harder)
}