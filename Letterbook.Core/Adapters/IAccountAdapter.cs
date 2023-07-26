using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IAccountAdapter
{
    public bool RecordAccount(Account account);
    public bool RecordAccounts(IEnumerable<Account> accounts);
}