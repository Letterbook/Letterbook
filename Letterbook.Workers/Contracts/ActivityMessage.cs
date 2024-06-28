using System.Collections.Immutable;
using System.Security.Claims;
using Medo;

namespace Letterbook.Workers.Contracts;

public record ActivityMessage
{
	public Uuid7? OnBehalfOf { get; init; }
	public required Uri Inbox { get; init; }
	public required string NextData { get; init; }
	public string? PrevData { get; init; }
	public required string Subject { get; init; }
	public required string Type { get; init; }
	public required ImmutableHashSet<Claim> Claims { get; init; }
}