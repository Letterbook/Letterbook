using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

public interface IAccountProfileAdapter : IDisposable
{
	delegate IQueryable<Profile> ProfileQuery(IQueryable<Profile> querySource);

	delegate IQueryable<Account> AccountQuery(IQueryable<Account> querySource);

	delegate bool ProfileComparer(Profile profile);

	public bool RecordAccount(Account account);
	public Task<bool> RecordAccounts(IEnumerable<Account> accounts);
	public Task<Account?> LookupAccount(Guid id);
	public Task<Account?> FindAccountByEmail(string normalizedEmail);

	/// <summary>
	/// Query for Accounts
	/// </summary>
	/// <returns></returns>
	public IQueryable<Account> SearchAccounts();

	/// <summary>
	/// Compose a query including LinkedProfiles
	/// </summary>
	/// <param name="query">The query to compose onto</param>
	/// <returns></returns>
	public IQueryable<Account> WithProfiles(IQueryable<Account> query);

	public Task<bool> AnyProfile(string handle);
	public Task<Profile?> LookupProfile(Uuid7 localId);
	public Task<Profile?> LookupProfile(Uri id);
	public IQueryable<Profile> SingleProfile(Uuid7 id);
	public IQueryable<Profile> SingleProfile(Uri fediId);
	public IQueryable<Profile> WithAudience(IQueryable<Profile> query);
	public IQueryable<Profile> WithRelation(IQueryable<Profile> query, Uri relationId);

	// Lookup Profile including relations to another profile
	[Obsolete("Use IQueryable methods", false)]
	public Task<Profile?> LookupProfileWithRelation(Uri id, Uri relationId);
	[Obsolete("Use IQueryable methods", false)]
	public Task<Profile?> LookupProfileWithRelation(Uuid7 localId, Uri relationId);

	public IAsyncEnumerable<Profile> FindProfilesByHandle(string handle, bool partial = false, int limit = 20, int page = 0);

	public void Add(Profile profile);
	public void Add(Account account);
	public void AddRange(IEnumerable<Profile> profile);
	public void AddRange(IEnumerable<Account> account);
	public void Update(Profile profile);
	public void Update(Account account);
	public void UpdateRange(IEnumerable<Profile> profile);
	public void UpdateRange(IEnumerable<Account> account);
	public void Delete(object record);

	public Task Cancel();
	public Task Commit();
}