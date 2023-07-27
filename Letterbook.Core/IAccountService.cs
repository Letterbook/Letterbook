using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IAccountService
{
    public Account? RegisterAccount(string email, string username);
    public Account? LookupAccount(string id);
    public IEnumerable<Account> FindAccounts(string email);
    public IEnumerable<Account> FindRelatedAccounts(Profile profile);
    public bool UpdateAccount(Account account);
}