using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IAccountProfileAdapter
{
    public bool RecordAccount(Account account);
    public bool RecordAccounts(IEnumerable<Account> accounts);
    public Account? LookupAccount(string id);
    public Profile? LookupProfile(string localId);
}