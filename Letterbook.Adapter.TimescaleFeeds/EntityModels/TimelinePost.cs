using System.Diagnostics;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.TimescaleFeeds.EntityModels;

/// <summary>
/// A partial view of a Post, adapted for use and query performance in Timelines
/// </summary>
[DebuggerDisplay("PostId: {PostId}, AudienceId: {AudienceId}")]
public class TimelinePost
{
	/// <summary>
	/// Time is the timeseries key. TSDBs will time-order everything and do rapid time-indexed queries.
	/// In our case, records should generally have a time value of right now, because what we care about is when it got added to
	/// the timeline. When it was added could and likely will be different from any of the datetime properties on Post
	/// </summary>
	public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

	/// <summary>
	/// Id of the Post
	/// </summary>
	public required Guid PostId { get; set; }

	/// <summary>Same as <see cref="Post"/></summary>
	public required string Preview { get; set; }

	/// <summary>Same as <see cref="Post"/></summary>
	public required string Authority { get; set; }

	/// <summary>Same as <see cref="Post"/></summary>
	public List<TimelineProfile> Creators { get; set; } = new();

	/// <summary>Same as <see cref="Post"/></summary>
	public string? Summary { get; set; }

	/// <summary>Same as <see cref="Post"/></summary>
	public required DateTimeOffset UpdatedDate { get; set; }

	/// PostId of the InReplyTo Post <summary>Same as <see cref="Post"/></summary>
	public Guid? InReplyToId { get; set; }

	/// <summary>
	/// Posts can be shared by multiple profiles, but each share would result in at-most one addition to the timeline.
	/// So TimelinePost has a single SharedBy property
	/// </summary>
	public TimelineProfile? SharedBy { get; set; }

	/// <summary>
	/// Posts can, and often will, be available to multiple Audiences. Posts with multiple Audiences will be duplicated
	/// in the Timeline table, with one record for each Audience.
	/// This allows us to query for specific memberships using WHERE IN (list) clauses, rather than requiring joins or
	/// complex set intersections
	/// </summary>
	public required Uri AudienceId { get; set; }

	/// <summary>ID of the Thread to which the Post belongs</summary>
	public required Guid ThreadId { get; set; }

	public static implicit operator Post(TimelinePost p) => new Post
	{
		Id = p.PostId,
		Thread = new ThreadContext()
		{
			Id = p.ThreadId,
			FediId = default!,
			RootId = p.PostId,
			Heuristics = new Heuristics() { Source = Heuristics.Origin.Timeline }
		},
		Summary = null,
		Preview = p.Preview,
		Creators = p.Creators.Select(pc => (Profile)pc).ToList(),
		UpdatedDate = p.UpdatedDate,
		Audience = [],
		InReplyTo = p.InReplyToId != null ? new Post() { Id = p.InReplyToId.Value } : default,
	};

	public static IEnumerable<TimelinePost> Denormalize(Post p, Profile? sharedBy = null)
	{
		return p.Audience
			.Concat(p.AddressedTo.Select(mention => Audience.FromMention(mention.Subject)))
			// Remove the public audience, because we will never query for it.
			.Where(a => a != Audience.Public)
			.Select(audience => new TimelinePost
		{
			PostId = p.Id,
			Preview = p.Preview ?? "PREVIEW NOT AVAILABLE", // TODO: Generate Preview
			Authority = p.Authority,
			Creators = p.Creators.Select(pc => (TimelineProfile)pc).ToList(),
			Summary = p.Summary,
			UpdatedDate = p.UpdatedDate ?? p.CreatedDate,
			InReplyToId = p.InReplyTo?.Id,
			SharedBy = sharedBy != null ? (TimelineProfile)sharedBy : default,
			AudienceId = audience.FediId,
			ThreadId = p.Thread.Id
		});
	}

	public static IEnumerable<TimelinePost> Denormalize(IEnumerable<Post> posts) =>
		posts.SelectMany(p => Denormalize(p));
}