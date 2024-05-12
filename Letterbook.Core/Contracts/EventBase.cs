using System.Reflection;
using System.Security.Claims;

namespace Letterbook.Core.Contracts;

public abstract record EventBase<T>
{
	public string? Source { get; init; } = Assembly.GetEntryAssembly()?.FullName;
	public required T Data { get; init; }
	public T? Updated { get; init; }
	public required string Type { get; init; }
	public required string Subject { get; init; }
	public DateTimeOffset Time { get; init; } = DateTimeOffset.UtcNow;
	public required IEnumerable<Claim> Claims { get; init; }

	// Id = Guid.NewGuid().ToString(),
	// Source = _options.BaseUri(),
	// Data = values,
	// Type = $"{nameof(AccountEventService)}.{values.updated.GetType()}.{action}",
	// Subject = values.updated.Id.ToString(),
	// Time = DateTimeOffset.UtcNow,
}