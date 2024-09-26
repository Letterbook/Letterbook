using System.Collections.Immutable;
using System.Security.Claims;
using Medo;

namespace Letterbook.Workers.Contracts;

public record ActivityMessage
{
	public Uuid7? OnBehalfOf { get; init; }
	public required Uri Inbox { get; init; }
	public required string Data { get; init; }
	public required string Activity { get; init; }
	public required string Type { get; init; }
	public required Claim[] Claims { get; init; }
}