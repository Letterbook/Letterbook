namespace Letterbook.Core.Models;

public class Audience
{
    public Uri? Id { get; set; }
    public List<Actor> Members { get; set; }
    public List<ApObject> Objects { get; set; }
}