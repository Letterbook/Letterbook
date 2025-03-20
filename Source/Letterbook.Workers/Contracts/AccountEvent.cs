using System.Collections.Immutable;
using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Workers.Contracts;

public record AccountEvent
{
	public required Account NextData { get; init; }
	public Account? PrevData { get; init; }
	public required string Subject { get; init; }
	public required string Type { get; init; }
	public required Claim[] Claims { get; init; }
}