using Letterbook.Core.Exceptions;
using Medo;

namespace Letterbook.Core.Models;

public class Post
{
    public Post()
    {
        Id = Uuid7.NewGuid();
        ContentRootId = Uuid7.Empty!;
        IdUri = default!;
        Thread = default!;
    }
    
    public Uuid7 Id { get; set; }
    public required Uuid7 ContentRootId { get; set; }
    public required Uri IdUri { get; set; }
    public ThreadContext Thread { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public string? Source { get; set; }
    public string Hostname => IdUri.Host;

    /// <summary>
    /// Authority is preprocessed this way for easy instance level moderation. It puts the host in reverse dns order.
    /// This would permit us to do fast prefix matches when filtering whole domains.
    /// Ex:
    /// blocking social.truth.*
    /// easily covers truth.social, and also block-evasion.truth.social, and truth.social:8443
    /// </summary>
    public string Authority =>
        IdUri.IsDefaultPort
            ? string.Join('.', IdUri.Host.Split('.').Reverse())
            : string.Join('.', IdUri.Host.Split('.').Reverse()) + IdUri.Port;
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

    // TODO: Post Factory
    public static Post Create<T>(Profile creator, T content) where T : Content
    {
        throw new NotImplementedException();
    }

    public static Post Create<T>(Profile creator, string? summary = null, string? preview = null, Uri? source = null)
        where T : Content
    {
        // TODO: Canonical
        var canonicalUri = new Uri("");
        return Create(creator, NewContent<T>(canonicalUri, summary, preview, source));
    }

    public T AddContent<T>(Uri canonicalUri, string? summary = null, string? preview = null, Uri? source = null)
        where T : Content
    {
        var t = NewContent<T>(canonicalUri, summary, preview, source);
        return AddContent(t);
    }

    public T AddContent<T>(T content) where T : Content
    {
        Contents.Add(content);
        return content;
    }

    private static T NewContent<T>(Uri canonicalUri, string? summary = null, string? preview = null, Uri? source = null)
        where T : class, IContent
    {
        if (Activator.CreateInstance(typeof(T), true) is not T t) 
            throw CoreException.InternalError($"Can't create Content type {typeof(T)}");
        t.IdUri = canonicalUri;
        t.Summary = summary;
        t.Preview = preview;
        t.Source = source;

        return t;
    }
}