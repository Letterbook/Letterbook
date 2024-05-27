using System.Collections.Immutable;
using System.Security.Claims;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Workers.Contracts;

public record PostEvent
{
	public Uuid7? Sender { get; init; }
	public required Post NextData { get; init; }
	public Post? PrevData { get; init; }
	public required string Subject { get; init; }
	public required Claim[] Claims { get; init; }
	public string Type { get; init; }

}