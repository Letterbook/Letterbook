using System.Linq.Expressions;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

public interface IAccountProfileAdapter : IDisposable
{
	delegate IQueryable<Profile> ProfileQuery(IQueryable<Profile> querySource);

	delegate IQueryable<Account> AccountQuery(IQueryable<Account> querySource);

	delegate bool ProfileComparer(Profile profile);

	/// <summary>
	/// Query for one account by ID
	/// </summary>
	/// <param name="accountId"></param>
	/// <returns></returns>
	IQueryable<Account> SingleAccount(Guid accountId);

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

	/// <summary>
	/// Include follow relationships in the profile query
	/// </summary>
	/// <param name="query"></param>
	/// <param name="relationId"></param>
	/// <returns></returns>
	public IQueryable<Profile> WithRelation(IQueryable<Profile> query, Uri relationId);

	/// <see cref="WithRelation(System.Linq.IQueryable{Letterbook.Core.Models.Profile},System.Uri)"/>
	public IQueryable<Profile> WithRelation(IQueryable<Profile> query, Uuid7 relationId);

	/// <summary>
	/// Query for navigation entities, using the given profile as a root entity
	/// </summary>
	/// <remarks>Consumers should be sure to set an OrderBy property</remarks>
	/// <param name="profile"></param>
	/// <param name="queryExpression">An expression func that specifies the navigation property to query</param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public IQueryable<T> QueryFrom<T>(Profile profile, Expression<Func<Profile, IEnumerable<T>>> queryExpression)
		where T : class;

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