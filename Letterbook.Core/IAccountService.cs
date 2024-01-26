using System.Security.Claims;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Letterbook.Core;

public interface IAccountService
{
    public Task<IList<Claim>> AuthenticatePassword(string email, string password);
    public Task<IdentityResult> RegisterAccount(string email, string handle, string password);
    public Task<Account?> LookupAccount(Guid id);
    public Task<IEnumerable<Account>> FindAccounts(string email);
    public Task<bool> UpdateEmail(Guid accountId, string email);
    public Task<bool> AddLinkedProfile(Guid accountId, Profile profile, ProfilePermission permission);
    public Task<bool> UpdateLinkedProfile(Guid accountId, Profile profile, ProfilePermission permission);
    public Task<bool> RemoveLinkedProfile(Guid accountId, Profile profile);
}