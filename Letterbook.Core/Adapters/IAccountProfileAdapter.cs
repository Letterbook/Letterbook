using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IAccountProfileAdapter : IDisposable
{
    public bool RecordAccount(Account account);
    public Task<bool> RecordAccounts(IEnumerable<Account> accounts);
    public Task<Account?> LookupAccount(Guid id);
    public Task<Profile?> LookupProfile(string localId);
    public IQueryable<Account> SearchAccounts();
    
    public Task Cancel();
    public Task Commit();
}