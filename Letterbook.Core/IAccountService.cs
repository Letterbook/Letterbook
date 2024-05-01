using System.Security.Principal;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.Core;

public interface IAccountService
{
	public Task<AccountIdentity> AuthenticatePassword(string email, string password);
	public Task<IdentityResult> RegisterAccount(string email, string handle, string password);
	public Task<Account?> LookupAccount(Guid id);
	public Task<IEnumerable<Account>> FindAccounts(string email);
	public Task<bool> UpdateEmail(Guid accountId, string email);
	public Task<bool> AddLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims);
	public Task<bool> UpdateLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims);
	public Task<bool> RemoveLinkedProfile(Guid accountId, Profile profile);
}