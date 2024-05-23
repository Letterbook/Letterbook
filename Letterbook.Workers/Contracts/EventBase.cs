using System.Collections.Immutable;
using System.Reflection;
using System.Security.Claims;

namespace Letterbook.Workers.Contracts;

public abstract record EventBase<T> : EventBase
{
	public new required T NextData { get; init; }
	public new T? PrevData { get; init; }
}

public abstract record EventBase
{
	public object? NextData { get; init; }
	public object? PrevData { get; init; }
	public string? Source { get; init; } = Assembly.GetEntryAssembly()?.FullName;
	public required string Subject { get; init; }
	public DateTimeOffset Time { get; init; } = DateTimeOffset.UtcNow;
	public required ImmutableHashSet<Claim> Claims { get; init; }
	public required string Type { get; init; }
}