using System.Diagnostics.CodeAnalysis;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Generators;
using Medo;

namespace Letterbook.Core.Models;

public partial record struct PostId(Uuid7 Id) : ITypedId<Uuid7>;

public class Post : IFederated, IEquatable<Post>
{
	public Post()
	{
		Id = Uuid7.NewUuid7();
		ContentRootIdUri = default!;
		FediId = default!;
		Thread = default!;
		Authority = default!;
		Hostname = default!;
	}

	/// <summary>
	/// Construct a new federated post with an unknown ThreadContext
	/// </summary>
	/// <param name="fediId">FediId</param>
	/// <param name="thread">Thread</param>
	public Post(Uri fediId) : this()
	{
		FediId = fediId;
		Authority = fediId.GetAuthority();
		Hostname = fediId.Host;
		Thread = new ThreadContext
		{
			RootId = Id,
			FediId = fediId,
		};
	}

	/// <summary>
	/// Construct a new federated post with a known or presumed ThreadContext
	/// </summary>
	/// <param name="fediId">FediId</param>
	/// <param name="thread">Thread</param>
	[SetsRequiredMembers]
	public Post(Uri fediId, ThreadContext thread) : this(fediId)
	{
		Thread = thread;
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
	public Post(CoreOptions opts) : this()
	{
		var builder = new UriBuilder(opts.BaseUri());
		Id = Uuid7.NewUuid7();
		ContentRootIdUri = default!;

		builder.Path += $"post/{Id}";
		var path = builder.Path;
		FediId = builder.Uri;

		builder.Path += "/replies";
		Replies = builder.Uri;
		builder.Path = path + "/likes";
		Likes = builder.Uri;
		builder.Path = path + "/shares";
		Shares = builder.Uri;

		Thread = new ThreadContext(Id, opts);
		Thread.Posts.Add(this);
		Authority = FediId.GetAuthority();
		Hostname = FediId.Host;
	}

	public PostId Id { get; set; }

	public Uri? ContentRootIdUri { get; set; }
	public Uri FediId { get; set; }
	public ThreadContext Thread { get; set; }
	public string? Summary { get; set; }
	public string? Preview { get; set; }
	public string? Source { get; set; }
	public string Hostname { get; set; }
	public string Authority { get; set; }
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
	public IEnumerable<ModerationReport> RelatedReports { get; set; } = new HashSet<ModerationReport>();

	public Uuid7 GetId() => Id.Id;
	public string GetId25() => Id.ToString();
	public Post ShallowClone() => (Post)MemberwiseClone();

	public T AddContent<T>(T content) where T : Content
	{
		if (Contents.Count == 0) SetRootContent(content);
		Contents.Add(content);
		content.Post = this;
		return content;
	}

	public void Mention(Profile subject, MentionVisibility visibility)
	{
		AddressedTo.Add(new Mention(this, subject, visibility));
	}

	public void SetRootContent(Content content)
	{
		ContentRootIdUri = content.FediId;
	}

	public Content? GetRootContent() => Contents.Order().FirstOrDefault();

	/// <summary>
	/// Replace mentions in this post with e
	/// </summary>
	/// <param name="profiles"></param>
	/// <exception cref="CoreException"></exception>
	public void ConvergeMentions(Dictionary<ProfileId, Profile> profiles)
	{
		var missing = AddressedTo.Select(mention => mention.Subject.Id).Where(id => !profiles.ContainsKey(id)).ToHashSet();
		if (missing.Count != 0)
			throw CoreException.MissingData<Profile>($"These profiles do not exist [{string.Join(", ", missing)}]", missing);
		foreach (var mention in AddressedTo)
		{
			if (profiles.TryGetValue(mention.Subject.Id, out var profile))
			{
				mention.Subject = profile;
			}
		}
	}

	public IEnumerable<Uri> ConvergeMentions(Dictionary<Uri, Profile> profiles)
	{
		var missing = AddressedTo.Select(mention => mention.Subject.FediId).Where(id => !profiles.ContainsKey(id)).ToHashSet();
		foreach (var mention in AddressedTo)
		{
			if (profiles.TryGetValue(mention.Subject.FediId, out var profile))
			{
				mention.Subject = profile;
			}
		}

		return missing;
	}

	public override int GetHashCode()
	{
		return FediId.GetHashCode();
	}

	public bool Equals(Post? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Id.Equals(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((Post)obj);
	}

	public static bool operator ==(Post? left, Post? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(Post? left, Post? right)
	{
		return !Equals(left, right);
	}
}