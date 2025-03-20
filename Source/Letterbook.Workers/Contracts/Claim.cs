using OpenIddict.Abstractions;
using Claims = System.Security.Claims;

namespace Letterbook.Workers.Contracts;

/// <summary>
/// A DTO for Claims
/// </summary>
public class Claim
{
	public required string Type { get; init; }
	public required string Value { get; init; }
	public string? ValueType { get; init; }
	public string? Issuer { get; init; }

	public static implicit operator Claims.Claim(Claim claim) => new(claim.Type, claim.Value, claim.ValueType, claim.Issuer);
	public static implicit operator Claim(Claims.Claim claim) => new()
	{
		Type = claim.Type,
		Value = claim.Value,
		ValueType = claim.ValueType,
		Issuer = claim.Issuer
	};
}