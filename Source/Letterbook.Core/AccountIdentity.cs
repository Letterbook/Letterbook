using System.Security.Claims;
using Letterbook.Core.Models;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.Core;

public class AccountIdentity : ClaimsIdentity
{
	public bool Authenticated { get; init;  }
	public bool LockedOut { get; init; }
	public bool TwoFactorRequired { get; init; }

	public AccountIdentity(bool lockedOut)
	{
		Authenticated = false;
		LockedOut = lockedOut;
		TwoFactorRequired = false;
	}

	public AccountIdentity(Account account, bool twoFactorRequired) : base(account)
	{
		Authenticated = true;
		LockedOut = false;
		TwoFactorRequired = twoFactorRequired;

		base.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()));
		base.AddClaim(new Claim(JwtRegisteredClaimNames.Email, account.Email ?? ""));
		base.AddClaim(new Claim("email_confirmed", account.EmailConfirmed.ToString()));
		base.AddClaims(account.LinkedProfiles.Select(link => new Claim($"profile:{link.Profile.GetId25()}", string.Join(',', link.Claims))));
	}

	public static AccountIdentity Succeed(bool useTwoFactor, Account account) => new AccountIdentity(account, useTwoFactor);
	public static AccountIdentity Fail(bool lockout) => new AccountIdentity(lockout);
}