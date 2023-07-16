namespace Letterbook.Core.Models;

public class Note : IContentRef
{
    private Note()
    {
        Id = default!;
        Authority = default!;
        Content = default!;
        CreatedDate = default!;
    }
    public Uri Id { get; set; }
    public string? LocalId { get; set; }
    public Uri Authority { get; set; }
    public HashSet<Profile> Creators { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public string Content { get; set; }  // TODO: HTML encode & sanitize
    public string? Summary { get; set; } // TODO: strip all HTML
    public HashSet<Audience> Visibility { get; set; } = new();
    public HashSet<Mention> Mentions { get; set; } = new();
    public string? Client { get; set; }
    public Note? ReplyTo { get; set; }
    public List<Note> Replies { get; set; } = new();
    // tags?
}