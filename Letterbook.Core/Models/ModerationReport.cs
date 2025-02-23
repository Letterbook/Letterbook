using System.Diagnostics.CodeAnalysis;
using Letterbook.Core.Extensions;
using Letterbook.Generators;
using Medo;

namespace Letterbook.Core.Models;

public partial record struct ModerationReportId(Uuid7 Id) : ITypedId<Uuid7>;

public class ModerationReport
{
	public ModerationReportId Id { get; set; } = Uuid7.NewUuid7();
	public Uri? FediId { get; set; }
	public required string Summary { get; set; }
	public required ThreadContext Context { get; set; }
	public ICollection<Account> Moderators { get; set; } = new HashSet<Account>();
	public ICollection<ModerationPolicy> Policies { get; set; } = new HashSet<ModerationPolicy>();
	public Profile? Reporter { get; set; }
	public ICollection<Profile> Subjects { get; set; } = new HashSet<Profile>();
	public ICollection<Post> RelatedPosts { get; set; } = new HashSet<Post>();
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Closed { get; set; } = DateTimeOffset.MaxValue;
	public ICollection<ModerationRemark> Remarks { get; set; } = new List<ModerationRemark>();
	// TODO: Support forwarding to labelers
	public IList<Uri> Forwarded { get; set; } = new List<Uri>();

	private readonly Lazy<HashSet<Uri>> _lazyForwardingInboxes;

	public ModerationReport()
	{
		_lazyForwardingInboxes = new Lazy<HashSet<Uri>>(() => Subjects.Select(p => p.SharedInbox)
			.Concat(RelatedPosts.SelectMany(p => p.Creators).Select(p => p.SharedInbox))
			.WhereNotNull()
			.ToHashSet());
	}

	[SetsRequiredMembers]
	public ModerationReport(CoreOptions opts, string summary) : this()
	{
		Summary = summary;
		var builder = new UriBuilder(opts.BaseUri());
		builder.Path += $"report/{Id.ToString()}";
		FediId = builder.Uri;
		Context = new ThreadContext()
		{
			RootId = default(PostId),
			FediId = FediId
		};
	}

	public HashSet<Uri> ForwardingInboxes() => _lazyForwardingInboxes.Value;

	public bool ForwardTo(Uri inbox, bool resend = false)
	{
		if (!ForwardingInboxes().Contains(inbox)) return false;
		if (Forwarded.Contains(inbox)) return resend;
		Forwarded.Add(inbox);
		return true;
	}

	public bool Close()
	{
		if (Closed <= DateTimeOffset.UtcNow) return false;
		Closed = DateTimeOffset.UtcNow;
		return true;
	}

	public bool IsClosed() => Closed <= DateTimeOffset.UtcNow;
}