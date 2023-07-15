namespace Letterbook.Core.Models;

public class Audience
{
    private Audience()
    {
        Id = default!;
    }
    
    public Uri Id { get; set; }
    public List<Actor> Members { get; set; } = new();
    public List<ApObject> Objects { get; set; } = new();
}