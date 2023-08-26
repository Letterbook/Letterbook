    using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.Db;

public class AccountProfileAdapter : IAccountProfileAdapter
{
    public bool RecordAccount(Models.Account account)
    {
        throw new NotImplementedException();
    }

    public bool RecordAccounts(IEnumerable<Models.Account> accounts)
    {
        throw new NotImplementedException();
    }

    public Models.Account? LookupAccount(string id)
    {
        throw new NotImplementedException();
    }

    public Models.Profile? LookupProfile(string localId)
    {
        throw new NotImplementedException();
    }
}