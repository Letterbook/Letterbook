using Letterbook.Core.Adapters;
using Microsoft.EntityFrameworkCore;
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

    public bool RecordAccount(Models.Account account)
    {
        var added = _context.Accounts.Add(account);
        _logger.LogDebug("{Method} added properties {@Properties}", nameof(RecordAccount), added.Properties.Select(p => $"{p.Metadata.Name}:{p.CurrentValue}"));
        return added.State == EntityState.Added;
    }

    public Task<bool> RecordAccounts(IEnumerable<Models.Account> accounts)
    {
        throw new NotImplementedException();
    }

    public Models.Account? LookupAccount(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Models.Profile?> LookupProfile(string localId)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Models.Profile> SearchProfiles()
    {
        throw new NotImplementedException();
    }

    public bool RecordProfile(Models.Profile profile)
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
