namespace Letterbook.Core.Models;

public class Note : IContentRef
{
    private Note(ObjectCollection<Profile> creators, ObjectCollection<Audience> visibility, ObjectList<Note> replies, ObjectList<Profile> likedBy, ObjectList<Profile> boostedBy)
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
        Creators = ObjectCollection<Profile>.Creators(id);
        Visibility = ObjectCollection<Audience>.Audience(id);
        Replies = ObjectList<Note>.Replies(id);
        LikedBy = ObjectList<Profile>.Likes(id);
        BoostedBy = ObjectList<Profile>.Boosts(id);
        CreatedDate = DateTime.UtcNow;
    }

    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;
    public ObjectCollection<Profile> Creators { get; set; }
    public DateTime CreatedDate { get; set; }
    public ActivityObjectType Type => ActivityObjectType.Note;
    public string Content { get; set; } = string.Empty; // TODO: HTML encode & sanitize
    public string? Summary { get; set; } // TODO: strip all HTML
    public ObjectCollection<Audience> Visibility { get; set; }
    public ICollection<Mention> Mentions { get; set; } = new HashSet<Mention>();
    public string? Client { get; set; }
    public Note? InReplyTo { get; set; }
    public ObjectList<Note> Replies { get; set; }
    public ObjectList<Profile> LikedBy { get; set; }
    public ObjectList<Profile> BoostedBy { get; set; }

    // You may be wondering, what's the difference between Attachments and tags?
    // The answer is that the spec authors had good intentions, but at this point it's not clear.
    // Tags are intended to be references, and attachments are intended to be included, like an email attachment.
    // public IList<IContentRef> Attachments { get; set; }
    // public ICollection<IContentRef> Tags { get; set; }
}