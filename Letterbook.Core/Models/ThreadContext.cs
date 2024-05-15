using Medo;

namespace Letterbook.Core.Models;

public class ThreadContext
{
	private Uuid7 _id = Uuid7.NewUuid7();
	private Uuid7 _rootId;

	public Guid Id
	{
		get => _id.ToGuid();
		set => _id = Uuid7.FromGuid(value);
	}

	public required Guid RootId
	{
		get => _rootId.ToGuid();
		set => _rootId = Uuid7.FromGuid(value);
	}

	public required Uri FediId { get; set; }
	public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
	public Heuristics? Heuristics { get; init; }

	public Uuid7 GetId() => _id;
	public string GetId25() => _id.ToId25String();
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

