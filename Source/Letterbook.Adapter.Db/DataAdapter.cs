using System.Linq.Expressions;
using Letterbook.Core.Adapters;
using Medo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.Db;

public class DataAdapter : IDataAdapter, IAsyncDisposable
{
	private readonly ILogger<DataAdapter> _logger;
	private readonly RelationalContext _context;

	public DataAdapter(ILogger<DataAdapter> logger, RelationalContext context)
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

	public IQueryable<Models.Account> Accounts(params Guid[] ids)
	{
		return ids.Length == 1
			? _context.Accounts.Where(a => a.Id == ids[0])
			: _context.Accounts.Where(a => ids.Contains(a.Id));
	}

	public IQueryable<Models.Account> AllAccounts()
	{
		return _context.Accounts.AsQueryable();
	}

	public IQueryable<Models.Post> Posts(params Models.PostId[] postIds)
	{
		return _context.Posts.Where(post => postIds.Contains(post.Id));
	}

	public IQueryable<Models.Post> Posts(params Uri[] postIds)
	{
		return _context.Posts.Where(post => postIds.Contains(post.FediId));
	}

	public IQueryable<Models.ThreadContext> Threads(Models.ThreadId id)
	{
		return _context.Threads.Where(thread => thread.Id == id);
	}

	public IQueryable<Models.ThreadContext> Threads(params Uri[] threadIds)
	{
		return threadIds.Length == 1
			? _context.Threads.Where(thread => thread.FediId == threadIds[0])
			: _context.Threads.Where(thread => threadIds.Contains(thread.FediId));
	}

	public IQueryable<Models.Profile> Profiles(params Uri[] ids)
	{
		return ids.Length == 1
			? _context.Profiles.Where(p => p.FediId == ids[0])
			: _context.Profiles.Where(p => ids.Contains(p.FediId));
	}

	public IQueryable<Models.Profile> Profiles(params Models.ProfileId[] ids)
	{
		return ids.Length == 1
			? _context.Profiles.Where(p => p.Id == ids[0])
			: _context.Profiles.Where(p => ids.Contains(p.Id));
	}

	public IQueryable<Models.Profile> AllProfiles() => _context.Profiles;

	public IQueryable<Models.ModerationReport> ModerationReports(params Models.ModerationReportId[] ids)
	{
		return ids.Length == 1
			? _context.ModerationReport.Where(p => p.Id == ids[0])
			: _context.ModerationReport.Where(p => ids.Contains(p.Id));
	}

	public IQueryable<Models.ModerationReport> ModerationReports(params Uri[] ids)
	{
		return ids.Length == 1
			? _context.ModerationReport.Where(p => p.FediId == ids[0])
			: _context.ModerationReport.Where(p => ids.Contains(p.FediId));
	}

	public IQueryable<Models.ModerationReport> AllModerationReports() => _context.ModerationReport;

	public IQueryable<Models.ModerationPolicy> Policies(params Models.ModerationPolicyId[] ids)
	{
		return ids.Length == 1
			? _context.ModerationPolicy.Where(p => p.Id == ids[0])
			: _context.ModerationPolicy.Where(p => ids.Contains(p.Id));
	}

	public IQueryable<Models.ModerationPolicy> AllPolicies() => _context.ModerationPolicy;

	public IQueryable<T> QueryFrom<T>(Models.Profile profile, Expression<Func<Models.Profile, IEnumerable<T>>> queryExpression)
		where T : class
	{
		return _context.Entry(profile).Collection(queryExpression).Query();
	}

	public IQueryable<T> QueryFrom<T>(Models.Post post, Expression<Func<Models.Post, IEnumerable<T>>> queryExpression) where T : class
	{
		return _context.Entry(post).Collection(queryExpression).Query();
	}

	public IQueryable<Models.Audience> Audiences(params Uri[] ids)
	{
		return ids.Length == 1
			? _context.Audience.Where(p => p.FediId == ids[0])
			: _context.Audience.Where(p => ids.Contains(p.FediId));
	}

	public IQueryable<Models.Audience> AllAudience()
	{
		return _context.Audience.AsQueryable();
	}

	public void Add(Models.Profile profile)
	{
		_context.Profiles.Add(profile);
	}

	public void Add(Models.Account account)
	{
		_context.Accounts.Add(account);
	}

	public void Add(Models.Post post) => _context.Posts.Add(post);

	public void Add(Models.ModerationReport report) => _context.ModerationReport.Add(report);
	public void Add(Models.ModerationPolicy policy) => _context.ModerationPolicy.Add(policy);
	public void Add(Models.InviteCode code) => _context.InviteCodes.Add(code);

	public void AddRange(IEnumerable<Models.Profile> profile)
	{
		_context.Profiles.AddRange(profile);
	}

	public void AddRange(IEnumerable<Models.Account> account)
	{
		_context.Accounts.AddRange(account);
	}

	public void AddRange(IEnumerable<Models.Post> posts)
	{
		_context.Posts.AddRange(posts);
	}

	public void Update(Models.Profile profile)
	{
		_context.Profiles.Update(profile);
	}

	public void Update(Models.Account account)
	{
		_context.Accounts.Update(account);
	}

	public void Update(Models.Post post) => _context.Posts.Update(post);

	public void Update(Models.Audience audience) => _context.Update(audience);
	public void Update(Models.InviteCode code) => _context.Update(code);

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

	public void Remove(Models.Post post) => _context.Posts.Remove(post);

	public void Remove(Models.Content content) => _context.Remove(content);

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

	private static IQueryable<Models.Post> WithDefaults(IQueryable<Models.Post> query) => query
		.Include(p => p.Creators)
		.Include(p => p.Audience)
		.Include(p => p.Contents);

	private static IQueryable<Models.Post> WithThread(IQueryable<Models.Post> query) => WithDefaults(query)
		.Include(post => post.Thread).ThenInclude(thread => thread.Posts).ThenInclude(p => p.Creators)
		.Include(post => post.Thread).ThenInclude(thread => thread.Posts).ThenInclude(p => p.Contents)
		.AsSplitQuery();

	public void Dispose()
	{
		_context.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await _context.DisposeAsync();
	}
}