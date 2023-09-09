using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.Db;

public class AccountProfileAdapter : IAccountProfileAdapter, IAsyncDisposable
{
    private readonly ILogger<AccountProfileAdapter> _logger;
    private readonly RelationalContext _context;

    public AccountProfileAdapter(ILogger<AccountProfileAdapter> logger, RelationalContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task<bool> RecordAccount(Models.Account account)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RecordAccounts(IEnumerable<Models.Account> accounts)
    {
        throw new NotImplementedException();
    }

    public Task<Models.Account?> LookupAccount(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Models.Profile?> LookupProfile(string localId)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Models.Account> SearchAccounts()
    {
        return _context.Accounts.AsQueryable();
    }
    
    public Task Cancel()
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            return _context.Database.RollbackTransactionAsync();
        }
        
        return Task.CompletedTask;
    }
    
    public Task Commit()
    {
        if (_context.Database.CurrentTransaction is not null)
        {
            return _context.Database.CommitTransactionAsync();
        }

        return _context.SaveChangesAsync();
    }

    private void Start()
    {
        if (_context.Database.CurrentTransaction is null)
        {
            _context.Database.BeginTransaction();
        }
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
