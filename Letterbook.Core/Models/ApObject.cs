namespace Letterbook.Core.Models;

public class ApObject
{
    public Uri Id { get; set; }
    public ActivityObjectType Type { get; set; }
    public string? LocalId { get; set; }
    public Uri Host { get; set; }
    public Actor Actor { get; set; }
    public List<Actor> AddressedTo { get; set; } = new List<Actor>();
    public List<Actor> AddressedBto { get; set; } = new List<Actor>();
    public List<Actor> AddressedCc { get; set; } = new List<Actor>();
    public List<Actor> AddressedBcc { get; set; } = new List<Actor>();
    public List<Audience> Audience { get; set; } = new List<Audience>();
    // TODO: content (languages are hard)
    // TODO: versions (ugh)
    // TODO: dates (versions make this harder)
}