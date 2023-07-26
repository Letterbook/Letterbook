using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.Db;

public class AccountAdapter : IAccountAdapter
{
    public bool RecordAccount(Models.Account account)
    {
        throw new NotImplementedException();
    }

    public bool RecordAccounts(IEnumerable<Models.Account> accounts)
    {
        throw new NotImplementedException();
    }
}