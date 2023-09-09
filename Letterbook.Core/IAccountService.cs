using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IAccountService
{
    public Task<ClaimsIdentity?> AuthenticatePassword(string email, string password);
    public Task<Account?> RegisterAccount(string email, string handle, string password);
    public Task<Account?> LookupAccount(string id);
    public Task<Profile> LookupProfile(string queryTarget);
    public Task<IEnumerable<Account>> FindAccounts(string email);
    public Task<bool> UpdateEmail(string accountId, string email);
    public Task<bool> AddLinkedProfile(string accountId, Profile profile, ProfilePermission permission);
    public Task<bool> UpdateLinkedProfile(string accountId, Profile profile, ProfilePermission permission);
    public Task<bool> RemoveLinkedProfile(string accountId, Profile profile);
}