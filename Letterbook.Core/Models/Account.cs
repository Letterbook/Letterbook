using System.Security.Claims;
using System.Security.Principal;
using Letterbook.Core.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Letterbook.Core.Models;

public class Account : IdentityUser<Guid>, IIdentity
{
	private string? _authenticationType = null;
	private bool _isAuthenticated = false;

	public string? AuthenticationType => _authenticationType;

	public bool IsAuthenticated => _isAuthenticated;

	public string? Name => UserName;
	public ICollection<ProfileClaims> LinkedProfiles { get; set; } = new HashSet<ProfileClaims>();

	public Account()
	{
		base.Id = Guid.NewGuid();
	}

	public IEnumerable<Claim> ProfileClaims(bool defaultActive = false)
	{
		if (LinkedProfiles is null) throw CoreException.MissingData<ProfileClaims>("LinkedProfiles not available", Id);
		var claims = LinkedProfiles
			.Where(linked => linked.Claims.Contains(ProfileClaim.Owner) || linked.Claims.Contains(ProfileClaim.Guest))
			.Select(claims => (Claim)claims);

		if (defaultActive && LinkedProfiles.FirstOrDefault(l => l.Claims.Contains(ProfileClaim.Owner)) is { } active)
			claims = claims.Append(new Claim(ApplicationClaims.ActiveProfile, active.Profile.GetId25()));

		return claims;
	}

	public Account ShallowClone() => (Account)MemberwiseClone();

	// In the future, Account might tie in to things like organizations or billing accounts

	// TODO(Account creation): https://github.com/Letterbook/Letterbook/issues/141
	public static Account CreateAccount(Uri baseUri, string email, string handle)
	{
		var profile = Profile.CreateIndividual(baseUri, handle);
		var account = new Account
		{
			Email = email,
			UserName = handle
		};
		profile.OwnedBy = account;
		account.LinkedProfiles.Add(new ProfileClaims(account, profile, [ProfileClaim.Owner]));

		return account;
	}

}
