using System.Security.Claims;
using System.Security.Principal;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.Core;

public interface IAccountService
{
	public Task<AccountIdentity> AuthenticatePassword(string email, string password);
	public Task<IdentityResult> ChangePassword(Guid id, string currentPassword, string newPassword);
	public Task<IdentityResult> RegisterAccount(string email, string handle, string password);
	public Task<Account?> LookupAccount(Guid id);

	/// <summary>
	/// Find all accounts matching the given email.
	/// <remarks>There should really only ever be one, but just in case</remarks>
	/// </summary>
	/// <param name="email"></param>
	/// <returns></returns>
	public IAsyncEnumerable<Account> FindAccounts(string email);

	/// <summary>
	/// Find a single account matching the given email.
	/// <remarks>This simplifies login, in particular</remarks>
	/// </summary>
	/// <param name="email"></param>
	/// <returns></returns>
	public Task<Account?> FirstAccount(string email);
	public Task<string> GenerateChangeEmailToken(Guid accountId, string email);

	/// <summary>
	/// Generate and deliver a password change
	/// </summary>
	/// <param name="email"></param>
	/// <param name="linkPartial">A redirect URL to be included in the delivered message. The token will be attached as a query parameter</param>
	/// <returns></returns>
	public Task DeliverPasswordChangeLink(string email, string linkPartial);
	public Task<IdentityResult> ChangeEmailWithToken(string oldEmail, string newEmail, string token);
	public Task<IdentityResult> ChangePasswordWithToken(string email, string password, string token);
	public Task<bool> AddLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims);
	public Task<bool> UpdateLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims);
	public Task<bool> RemoveLinkedProfile(Guid accountId, Profile profile);
	public Task<IEnumerable<Claim>> LookupClaims(Account account);
}