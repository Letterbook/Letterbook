using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IAccountService
{
    public Account? RegisterAccount(string email, string handle);
    public Account? LookupAccount(string id);
    public Profile LookupProfile(string queryTarget);
    public IEnumerable<Account> FindAccounts(string email);
    public bool UpdateEmail(string accountId, string email);
    public bool AddLinkedProfile(string accountId, Profile profile, ProfilePermission permission);
    public bool UpdateLinkedProfile(string accountId, Profile profile, ProfilePermission permission);
    public bool RemoveLinkedProfile(string accountId, Profile profile);
}