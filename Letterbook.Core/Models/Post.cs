namespace Letterbook.Core.Models;

public class Post
{
    private Post()
    {
        Uri = default!;
        Thread = default!;
    }
    
    public Guid Id { get; set; }
    public Uri Uri { get; set; }
    public Uri Thread { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public string? Source { get; set; }
    public string Hostname => Uri.Host;

    /// <summary>
    /// Authority is preprocessed this way for easy instance level moderation. It puts the host in reverse dns order.
    /// This would permit us to do fast prefix matches when filtering whole domains.
    /// Ex:
    /// blocking social.truth.*
    /// easily covers truth.social, and also block-evasion.truth.social, and truth.social:8443
    /// </summary>
    public string Authority =>
        Uri.IsDefaultPort
            ? string.Join('.', Uri.Host.Split('.').Reverse())
            : string.Join('.', Uri.Host.Split('.').Reverse()) + Uri.Port;
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