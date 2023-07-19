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
    public string Authority { get; set; }
    public ICollection<Profile> Creators { get; set; } = new HashSet<Profile>();
    public DateTime CreatedDate { get; set; }
    public string Content { get; set; }  // TODO: HTML encode & sanitize
    public string? Summary { get; set; } // TODO: strip all HTML
    public ICollection<Audience> Visibility { get; set; } = new HashSet<Audience>();
    public ICollection<Mention> Mentions { get; set; } = new HashSet<Mention>();
    public string? Client { get; set; }
    public Note? InReplyTo { get; set; }
    public IList<Note> Replies { get; set; } = new List<Note>();
    public ICollection<Profile> LikedBy { get; set; } = new HashSet<Profile>();
    public ICollection<Profile> BoostedBy { get; set; } = new HashSet<Profile>();

    // You may be wondering, what's the difference between Attachments and tags?
    // The answer is that the spec authors had good intentions, but at this point it's not clear.
    // Tags are intended to be references, and attachments are intended to be included, like an email attachment.
    // public IList<IContentRef> Attachments { get; set; }
    // public ICollection<IContentRef> Tags { get; set; }
}