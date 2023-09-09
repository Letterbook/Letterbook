using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IAccountProfileAdapter : IDisposable
{
    public Task<bool> RecordAccount(Account account);
    public Task<bool> RecordAccounts(IEnumerable<Account> accounts);
    public Task<Account?> LookupAccount(string id);
    public Task<Profile?> LookupProfile(string localId);
    public IQueryable<Account> SearchAccounts();
    
    public Task Cancel();
    public Task Commit();
}