using System.Reflection;
using System.Security.Claims;

namespace Letterbook.Core.Contracts;

public abstract record EventBase<T> : EventBase
{
	public new required T Data { get; init; }
	public new T? Updated { get; init; }
}

public abstract record EventBase
{
	public object? Data { get; init; }
	public object? Updated { get; init; }
	public string? Source { get; init; } = Assembly.GetEntryAssembly()?.FullName;
	public required string Type { get; init; }
	public required string Subject { get; init; }
	public DateTimeOffset Time { get; init; } = DateTimeOffset.UtcNow;
	public required IReadOnlySet<Claim> Claims { get; init; }
}