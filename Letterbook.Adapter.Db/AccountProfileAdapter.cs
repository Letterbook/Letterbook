using Letterbook.Core.Adapters;
using Medo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

	public IQueryable<Models.Account> SingleAccount(Guid accountId)
	{
		return _context.Accounts.Where(account => account.Id == accountId).Take(1).AsQueryable();
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

	public Task<Models.Account?> LookupAccount(Guid id)
	{
		return _context.Accounts.FirstOrDefaultAsync(account => account.Id == id);
	}

	public async Task<Models.Account?> FindAccountByEmail(string normalizedEmail)
	{
		return await _context.Accounts
			.Include(account => account.LinkedProfiles)
			.FirstOrDefaultAsync(account => account.NormalizedEmail == normalizedEmail);
	}

	public IQueryable<Models.Account> SearchAccounts()
	{
		return _context.Accounts.AsQueryable();
	}

	public IQueryable<Models.Account> WithProfiles(IQueryable<Models.Account> query)
	{
		return query
			.Include(account => account.LinkedProfiles)
			.AsSplitQuery();
	}


	public Task<bool> AnyProfile(string handle)
	{
		return _context.Profiles.AnyAsync(profile => profile.Handle == handle);
	}

	public Task<bool> AnyProfile(Uri id)
	{
		return _context.Profiles.AnyAsync(profile => profile.FediId == id);
	}

	public Task<Models.Profile?> LookupProfile(Uuid7 localId)
	{
		return _context.Profiles
			.Include(profile => profile.Keys)
			.AsSplitQuery()
			.FirstOrDefaultAsync(profile => profile.Id == localId.ToGuid());
	}

	public Task<Models.Profile?> LookupProfile(Uri id)
	{
		return _context.Profiles.FirstOrDefaultAsync(profile => profile.FediId == id);
	}

	public IQueryable<Models.Profile> SingleProfile(Uuid7 id)
	{
		return _context.Profiles.Where(profile => profile.Id == id.ToGuid())
			.Take(1);
	}

	public IQueryable<Models.Profile> SingleProfile(Uri fediId)
	{
		return _context.Profiles.Where(profile => profile.FediId == fediId)
			.Take(1);
	}

	public IQueryable<Models.Profile> WithAudience(IQueryable<Models.Profile> query)
	{
		return query.Include(profile => profile.Audiences);
	}

	public IQueryable<Models.Profile> WithRelation(IQueryable<Models.Profile> query, Uri relationId)
	{
		return query.Include(profile => profile.FollowingCollection.Where(relation => relation.Follows.FediId == relationId))
				.ThenInclude(relation => relation.Follows)
			.Include(profile => profile.FollowersCollection.Where(relation => relation.Follower.FediId == relationId))
				.ThenInclude(relation => relation.Follower)
			.Include(profile => profile.Keys)
			.Include(profile => profile.Audiences)
			.AsSplitQuery();
	}

	public Task<Models.Profile?> LookupProfileWithRelation(Uri id, Uri relationId)
	{
		return WithRelation(_context.Profiles.Where(profile => profile.FediId == id), relationId)
			.FirstOrDefaultAsync();
	}

	public Task<Models.Profile?> LookupProfileWithRelation(Uuid7 localId, Uri relationId)
	{
		return WithRelation(_context.Profiles.Where(profile => profile.Id == localId.ToGuid()), relationId)
			.FirstOrDefaultAsync();
	}

	public IAsyncEnumerable<Models.Profile> FindProfilesByHandle(string handle, bool partial = false, int limit = 20, int page = 0)
	{
		limit = limit >= 100 ? 100 : limit;
		var query = _context.Profiles.OrderBy(profile => profile.FediId)
			.Skip(limit * page)
			.Take(limit);
		query = partial
			? query.Where(profile => profile.Handle.StartsWith(handle))
			: query.Where(profile => profile.Handle == handle);
		return query.AsAsyncEnumerable();
	}

	public void Add(Models.Profile profile)
	{
		_context.Profiles.Add(profile);
	}

	public void Add(Models.Account account)
	{
		_context.Accounts.Add(account);
	}

	public void AddRange(IEnumerable<Models.Profile> profile)
	{
		_context.Profiles.AddRange(profile);
	}

	public void AddRange(IEnumerable<Models.Account> account)
	{
		_context.Accounts.AddRange(account);
	}

	public void Update(Models.Profile profile)
	{
		_context.Profiles.Update(profile);
	}

	public void Update(Models.Account account)
	{
		_context.Accounts.Update(account);
	}

	public void UpdateRange(IEnumerable<Models.Profile> profile)
	{
		_context.Profiles.UpdateRange(profile);
	}

	public void UpdateRange(IEnumerable<Models.Account> account)
	{
		_context.Accounts.UpdateRange(account);
	}

	public void Delete(object record)
	{
		if (record is EntityEntry entry)
		{
			entry.State = EntityState.Deleted;
		}
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