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

	public IQueryable<Models.Account> SingleAccount(Guid accountId)
	{
		return _context.Accounts.Where(account => account.Id == accountId).AsQueryable();
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

	public IQueryable<Models.Account> AllAccounts()
	{
		return _context.Accounts.AsQueryable();
	}

	public Task<bool> AnyProfile(string handle)
	{
		return _context.Profiles.AnyAsync(profile => profile.Handle == handle);
	}

	public Task<bool> AnyProfile(Uri id)
	{
		return _context.Profiles.AnyAsync(profile => profile.FediId == id);
	}

	public Task<Models.Profile?> LookupProfile(Models.ProfileId localId)
	{
		return _context.Profiles
			.Include(profile => profile.Keys)
			.AsSplitQuery()
			.FirstOrDefaultAsync(profile => profile.Id == localId);
	}

	public Task<Models.Profile?> LookupProfile(Uri id)
	{
		return _context.Profiles.FirstOrDefaultAsync(profile => profile.FediId == id);
	}

	public async Task<Models.Post?> LookupPost(Models.PostId postId)
	{
		return await WithDefaults(_context.Posts).FirstOrDefaultAsync(post => post.Id == postId);
	}

	public async Task<Models.Post?> LookupPost(Uri fediId)
	{
		return await WithDefaults(_context.Posts).FirstOrDefaultAsync(post => post.FediId == fediId);
	}

	public async Task<Models.ThreadContext?> LookupThread(Uri threadId)
	{
		return await _context.Threads
			.Include(thread => thread.Posts)
			.FirstOrDefaultAsync(thread => thread.FediId == threadId);
	}

	public async Task<Models.ThreadContext?> LookupThread(Models.ThreadId threadId)
	{
		return await _context.Threads
			.Include(thread => thread.Posts)
			.FirstOrDefaultAsync(thread => thread.Id == threadId);
	}

	public async Task<Models.Post?> LookupPostWithThread(Models.PostId postId)
	{
		return await WithThread(_context.Posts)
			.FirstOrDefaultAsync(post => post.Id == postId);
	}

	public async Task<Models.Post?> LookupPostWithThread(Uri postId)
	{
		return await WithThread(_context.Posts)
			.FirstOrDefaultAsync(post => post.FediId == postId);
	}

	public IQueryable<Models.Profile> SingleProfile(Models.ProfileId id) => Profiles(id);

	public IQueryable<Models.Profile> SingleProfile(Uri fediId) => Profiles(fediId);

	public IQueryable<Models.Profile> ListProfiles(IEnumerable<Models.ProfileId> profileIds)
	{
		return _context.Profiles.Where(post => profileIds.Contains(post.Id));
	}

	public IQueryable<Models.Profile> ListProfiles(IEnumerable<Uri> profileIds)
	{
		return _context.Profiles.Where(post => profileIds.Contains(post.FediId));
	}

	public IQueryable<Models.Post> SinglePost(Models.PostId id)
	{
		return _context.Posts.Where(post => post.Id == id);
	}

	public IQueryable<Models.Post> SinglePost(Uri fediId)
	{
		return _context.Posts.Where(post => post.FediId == fediId);
	}

	public IQueryable<Models.Post> Posts(IEnumerable<Models.PostId> postIds)
	{
		return _context.Posts.Where(post => postIds.Contains(post.Id));
	}

	public IQueryable<Models.Post> Posts(IEnumerable<Uri> postIds)
	{
		return _context.Posts.Where(post => postIds.Contains(post.FediId));
	}

	public IQueryable<Models.ThreadContext> Threads(Uuid7 id)
	{
		throw new NotImplementedException();
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

	public IQueryable<Models.Profile> WithRelation(IQueryable<Models.Profile> query, Models.ProfileId relationId)
	{
		return query.Include(profile => profile.FollowingCollection.Where(relation => relation.Follows.Id == relationId))
			.ThenInclude(relation => relation.Follows)
			.Include(profile => profile.FollowersCollection.Where(relation => relation.Follower.Id == relationId))
			.ThenInclude(relation => relation.Follower)
			.Include(profile => profile.Keys)
			.Include(profile => profile.Audiences)
			.AsSplitQuery();
	}

	public IQueryable<T> QueryFrom<T>(Models.Profile profile, Expression<Func<Models.Profile, IEnumerable<T>>> queryExpression)
		where T : class
	{
		return _context.Entry(profile).Collection(queryExpression).Query();
	}

	public IQueryable<T> QueryFrom<T>(Models.Post post, Expression<Func<Models.Post, IEnumerable<T>>> queryExpression) where T : class
	{
		return _context.Entry(post).Collection(queryExpression).Query();
	}

	public IQueryable<Models.Audience> AllAudience()
	{
		return _context.Audience.AsQueryable();
	}

	public Task<Models.Profile?> LookupProfileWithRelation(Models.ProfileId localId, Uri relationId)
	{
		return WithRelation(_context.Profiles.Where(profile => profile.Id == localId), relationId)
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

	public void Add(Models.Post post) => _context.Posts.Add(post);

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