using System.Security.Principal;
using Medo;
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

	public Account ShallowClone() => (Account)MemberwiseClone();

	public void Authenticate(string type)
	{
		_isAuthenticated = true;
		_authenticationType = type;
	}
}
