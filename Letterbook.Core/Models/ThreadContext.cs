using System.Diagnostics.CodeAnalysis;
using Letterbook.Core.Extensions;
using Letterbook.Generators;
using Medo;

namespace Letterbook.Core.Models;

public partial record struct ThreadId(Uuid7 Id) : ITypedId<Uuid7>;

public class ThreadContext
{
	public ThreadId Id { get; set; } = Uuid7.NewUuid7();
	public required PostId? RootId { get; set; }

	public Uri? FediId { get; set; }
	public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
	public Heuristics? Heuristics { get; init; }

	public ThreadContext() { }

	[SetsRequiredMembers]
	public ThreadContext(PostId rootId, CoreOptions opts)
	{
		var builder = new UriBuilder(opts.BaseUri());
		builder.Path += $"thread/{Id.ToString()}";
		FediId = builder.Uri;
		RootId = rootId;
	}

	public Uuid7 GetId() => Id.Id;
}

/// <summary>
/// Heuristics are indicators from the source ActivityPub message that may help in locating the correct ThreadContext
/// for an inbound post
/// </summary>
public class Heuristics
{
	/// <summary>
	/// The collection indicated by the source `context` property
	/// </summary>
	public Uri? Context;

	/// <summary>
	/// The likely root post of the thread
	/// </summary>
	public Uri? Root;

	/// <summary>
	/// The collection indicated by the source `thread` property
	/// </summary>
	public Uri? Target;

	/// <summary>
	/// The post is likely the root of its own thread, and the context can be used as-is
	/// </summary>
	public bool NewThread;

	/// <summary>
	/// Origin of the ThreadContext
	/// </summary>
	public Origin Source;

	public enum Origin
	{
		ActivityPub,
		Timeline
	}
}