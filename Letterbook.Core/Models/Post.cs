using System.Diagnostics.CodeAnalysis;
using Letterbook.Core.Exceptions;
using Medo;

namespace Letterbook.Core.Models;

public class Post : IFederated
{
    public Post()
    {
        Id = Uuid7.NewGuid();
        ContentRootIdUri = default!;
        FediId = default!;
        Thread = default!;
    }

    [SetsRequiredMembers]
    public Post(Uri fediId, ThreadContext thread) : this()
    {
        FediId = fediId;
        Thread = thread;
    }

    public Uuid7 Id { get; set; }
    public Uri ContentRootIdUri { get; set; }
    public required Uri FediId { get; set; }
    public ThreadContext Thread { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public string? Source { get; set; }
    public string Hostname => FediId.Host;

    /// <summary>
    /// Authority is preprocessed this way for easy instance level moderation. It puts the host in reverse dns order.
    /// This would permit us to do fast prefix matches when filtering whole domains.
    /// Ex:
    /// blocking social.truth.*
    /// easily covers truth.social, and also block-evasion.truth.social, and truth.social:8443
    /// </summary>
    public string Authority =>
        FediId.IsDefaultPort
            ? string.Join('.', FediId.Host.Split('.').Reverse())
            : string.Join('.', FediId.Host.Split('.').Reverse()) + FediId.Port;
    public ICollection<Profile> Creators { get; set; } = new HashSet<Profile>();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public ICollection<Content> Contents { get; set; } = new HashSet<Content>();
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

    public T AddContent<T>(T content) where T : Content
    {
        if (Contents.Count == 0) ContentRootIdUri = content.FediId;
        Contents.Add(content);
        return content;
    }
}