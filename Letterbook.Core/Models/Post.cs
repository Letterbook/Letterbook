namespace Letterbook.Core.Models;

public class Post
{
    private Post()
    {
        Id = default!;
    }

    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;
    public ICollection<Profile> Creators { get; set; } = new HashSet<Profile>();
    public DateTime CreatedDate { get; set; }
    public ICollection<IContent> Contents { get; set; } = new HashSet<IContent>();
    public ICollection<Audience> Audience { get; set; } = new HashSet<Audience>();
    public ICollection<Mention> AddressedTo { get; set; } = new HashSet<Mention>();
    public string? Client { get; set; }
    public Post? InReplyTo { get; set; }
    public Uri? Replies { get; set; }
    public IList<Post> RepliesCollection { get; set; } = new List<Post>();
    public Uri? Likes { get; set; }
    public IList<Profile> LikesCollection { get; set; } = new List<Profile>();
    public Uri? Shares { get; set; }
    public IList<Profile> SharesCollection { get; set; } = new List<Profile>();
}

public interface IContent
{
}