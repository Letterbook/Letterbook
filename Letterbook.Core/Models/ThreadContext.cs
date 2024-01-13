using Medo;

namespace Letterbook.Core.Models;

public class ThreadContext
{
    public Uuid7 Id { get; set; } = Uuid7.NewUuid7();
    public required Uri FediId { get; set; }
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    public required Post Root { get; set; }
    public Heuristics? Heuristics { get; init; }
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
}