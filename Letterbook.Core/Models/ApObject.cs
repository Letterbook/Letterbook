namespace Letterbook.Core.Models;

public class ApObject
{
    private ApObject()
    {
        Id = default!;
        Type = ActivityObjectType.None;
        Host = default!;
        Profile = default!;
    }

    public Uri Id { get; set; }
    public ActivityObjectType Type { get; set; }
    public string? LocalId { get; set; }
    public Uri Host { get; set; }
    public Profile Profile { get; set; }
    public List<Profile> AddressedTo { get; set; } = new();
    public List<Profile> AddressedBto { get; set; } = new();
    public List<Profile> AddressedCc { get; set; } = new();
    public List<Profile> AddressedBcc { get; set; } = new();
    public List<Audience> Audience { get; set; } = new();
    // TODO: content (languages are hard)
    // TODO: versions (ugh)
    // TODO: dates (versions make this harder)
}