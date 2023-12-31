using System.Collections;

namespace Letterbook.Core.Models;

public class Note : IContentRef
{
    private Note(ICollection<Profile> creators, ICollection<Audience> visibility, IList<Note> replies, IList<Profile> likedBy, IList<Profile> boostedBy)
    {
        Creators = creators;
        Visibility = visibility;
        Replies = replies;
        LikedBy = likedBy;
        BoostedBy = boostedBy;
        Id = default!;
        Content = default!;
        CreatedDate = default!;
    }

    public Note(Uri id)
    {
        Id = id;
        CreatedDate = DateTime.UtcNow;
    }

    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;
    public ICollection<Profile> Creators { get; set; } = new HashSet<Profile>();
    public DateTime CreatedDate { get; set; }
    public ActivityObjectType Type => ActivityObjectType.Note;
    public string Content { get; set; } = string.Empty; // TODO: HTML encode & sanitize
    public string? Summary { get; set; } // TODO: strip all HTML
    public ICollection<Audience> Visibility { get; set; } = new HashSet<Audience>();
    public ICollection<Mention> Mentions { get; set; } = new HashSet<Mention>();
    public string? Client { get; set; }
    public Note? InReplyTo { get; set; }
    public IList<Note> Replies { get; set; } = new List<Note>();
    public IList<Profile> LikedBy { get; set; } = new List<Profile>();
    public IList<Profile> BoostedBy { get; set; } = new List<Profile>();

    // You may be wondering, what's the difference between Attachments and tags?
    // The answer is that the spec authors had good intentions, but at this point it's not clear.
    // Tags are intended to be references, and attachments are intended to be included, like an email attachment.
    // public IList<IContentRef> Attachments { get; set; }
    // public ICollection<IContentRef> Tags { get; set; }
}