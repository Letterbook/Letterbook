using Medo;
using Microsoft.AspNetCore.Routing;
using Letterbook.Generators;

namespace Letterbook.Core.Models;

[GenerateTypedId]
public partial record struct InviteCodeId(Uuid7 Id) : IParameterPolicy { }

/// <summary>
/// Invite codes are used during account creation, as an authorization token when the server is configured to require them.
/// Each code may be valid for only a limited duration, or for a set number of uses. Codes are a 12-digit series of numbers and
/// (english) letters, which results in an approximately 62-bit key space. This means that collisions should be rare, but
/// realistically possible (unlike UUIDs). But that is acceptable, in exchange for being reasonably legible and typable.
/// Codes don't need to be unique, although it's better to try to minimize collisions within their validity period.
/// </summary>
public class InviteCode
{
	public InviteCodeId Id { get; set; } = Uuid7.NewUuid7();
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Expiration { get; set; } = DateTimeOffset.MaxValue;
	public int RemainingUses { get; set; }
	public int Uses { get; set; } = 1;
	public string Code { get; set; }
	public Account? CreatedBy { get; set; }
	public ICollection<Account> RedeemedBy { get; set; } = new List<Account>();

	public InviteCode(IInviteCodeGenerator code)
	{
		Code = code.Generate();
		RemainingUses = Uses;
	}

	public InviteCode(string code)
	{
		Code = code;
		RemainingUses = Uses;
	}

	/// <summary>
	/// Try to redeem the invite code
	/// </summary>
	/// <returns>True if the code was redeemable, false otherwise</returns>
	public bool TryRedeem(Account holder)
	{
		var limitedUses = Uses > 0;
		if (limitedUses && RemainingUses <= 0) return false;
		if (DateTimeOffset.UtcNow >= Expiration) return false;
		if (limitedUses) RemainingUses--;
		holder.InvitedBy = this;
		return true;
	}

	/// <summary>
	/// Revoke the code, make it unusable
	/// </summary>
	public void Revoke()
	{
		RemainingUses = 0;
		Expiration = DateTimeOffset.UtcNow;
	}
}