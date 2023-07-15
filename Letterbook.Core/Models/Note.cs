namespace Letterbook.Core.Models;

public class Note : IObjectRef
{
    public Uri Id { get; set; }
    public string? LocalId { get; set; }
    public Uri Authority { get; set; }
    public Actor Creator { get; set; }
    public string Content { get; set; }  // TODO: HTML encode & sanitize
    public string? Summary { get; set; } // TODO: HTML encode & strip all HTML
    public HashSet<Audience> Visibility { get; set; }
    public HashSet<Mention> Mentions { get; set; }
    // tags?
}