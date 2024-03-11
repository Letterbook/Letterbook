using System.Diagnostics.CodeAnalysis;
using Letterbook.Core.Extensions;
using Medo;

namespace Letterbook.Core.Models;

public class Post : IFederated
{
    private Uuid7 _id;

    public Post()
    {
        _id = Uuid7.NewUuid7();
        ContentRootIdUri = default!;
        FediId = default!;
        Thread = default!;
        Authority = default!;
        Hostname = default!;
    }

    /// <summary>
    /// Construct a new federated post with a known or presumed ThreadContext
    /// </summary>
    /// <param name="fediId">FediId</param>
    /// <param name="thread">Thread</param>
    [SetsRequiredMembers]
    public Post(Uri fediId, ThreadContext thread) : this()
    {
        FediId = fediId;
        Thread = thread;
        Authority = fediId.GetAuthority();
        Hostname = fediId.Host;
    }

    /// <summary>
    /// Construct a new local reply post in an existing thread
    /// </summary>
    /// <param name="opts"></param>
    /// <param name="parent">Post </param>
    [SetsRequiredMembers]
    public Post(CoreOptions opts, Post parent) : this(opts)
    {
        InReplyTo = parent;

        Thread = parent.Thread;
        Thread.Posts.Add(this);
        parent.RepliesCollection.Add(this);
    }

    /// <summary>
    /// Construct a new local top level Post
    /// </summary>
    /// <param name="opts"></param>
    [SetsRequiredMembers]
    public Post(CoreOptions opts)
    {
        var builder = new UriBuilder(opts.BaseUri());
        _id = Uuid7.NewUuid7();
        ContentRootIdUri = default!;

        builder.Path += $"post/{_id.ToId25String()}";
        FediId = builder.Uri;

        builder.Path += "/replies";
        Thread = new ThreadContext
        {
            FediId = builder.Uri,
            RootId = Id,
        };
        Thread.Posts.Add(this);
        Authority = FediId.GetAuthority();
        Hostname = FediId.Host;
    }

    public Guid Id
    {
        get => _id.ToGuid();
        set => _id = Uuid7.FromGuid(value);
    }

    public Uri? ContentRootIdUri { get; set; }
    public Uri FediId { get; set; }
    public ThreadContext Thread { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public string? Source { get; set; }
    public string Hostname { get; private set; }
    public string Authority { get; private set; }
    public ICollection<Profile> Creators { get; set; } = new HashSet<Profile>();
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PublishedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public DateTimeOffset LastSeenDate { get; set; } = DateTimeOffset.UtcNow;
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

    public Uuid7 GetId() => _id;
    public string GetId25() => _id.ToId25String();

    public T AddContent<T>(T content) where T : Content
    {
        if (Contents.Count == 0) SetRootContent(content);
        Contents.Add(content);
        return content;
    }

    public void SetRootContent(Content content)
    {
        ContentRootIdUri = content.FediId;
        FediId = content.FediId;
    }

    public override int GetHashCode()
    {
        return FediId.GetHashCode();
    }
}