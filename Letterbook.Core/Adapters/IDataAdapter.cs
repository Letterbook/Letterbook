using System.Linq.Expressions;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

public interface IDataAdapter : IDisposable
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
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions")]
	public IQueryable<Account> WithProfiles(IQueryable<Account> query);

	public Task<bool> AnyProfile(string handle);
	public Task<Profile?> LookupProfile(ProfileId localId);
	public Task<Profile?> LookupProfile(Uri id);

	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public Task<Post?> LookupPost(PostId postId);
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public Task<Post?> LookupPost(Uri fediId);
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public Task<ThreadContext?> LookupThread(Uri threadId);
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public Task<ThreadContext?> LookupThread(Uuid7 threadId);
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public Task<Post?> LookupPostWithThread(PostId postId);
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public Task<Post?> LookupPostWithThread(Uri postId);

	public IQueryable<Profile> SingleProfile(ProfileId id);
	public IQueryable<Profile> SingleProfile(Uri fediId);

	/// <summary>
	/// Query for a Post by Id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public IQueryable<Post> SinglePost(PostId id);

	/// <see cref="SinglePost(Medo.Uuid7)"/>
	public IQueryable<Post> SinglePost(Uri fediId);

	/// <summary>
	/// Query for a Thread by Id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public IQueryable<ThreadContext> SingleThread(Uuid7 id);
	/// <see cref="SingleThread(Medo.Uuid7)"/>
	public IQueryable<ThreadContext> SingleThread(Uri fediId);

	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public IQueryable<Profile> WithAudience(IQueryable<Profile> query);

	/// <summary>
	/// Include follow relationships in the profile query
	/// </summary>
	/// <param name="query"></param>
	/// <param name="relationId"></param>
	/// <returns></returns>
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public IQueryable<Profile> WithRelation(IQueryable<Profile> query, Uri relationId);

	/// <see cref="WithRelation(System.Linq.IQueryable{Letterbook.Core.Models.Profile},System.Uri)"/>
	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public IQueryable<Profile> WithRelation(IQueryable<Profile> query, ProfileId relationId);

	/// <summary>
	/// Query for navigation entities, using the given object as a root entity
	/// </summary>
	/// <remarks>Consumers should be sure to set an OrderBy property</remarks>
	/// <param name="profile"></param>
	/// <param name="queryExpression">An expression func that specifies the navigation property to query</param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public IQueryable<T> QueryFrom<T>(Profile profile, Expression<Func<Profile, IEnumerable<T>>> queryExpression)
		where T : class;

	public IQueryable<T> QueryFrom<T>(Post post, Expression<Func<Post, IEnumerable<T>>> queryExpression)
		where T : class;

	[Obsolete("Use Letterbook.Core.Extensions.QueryExtensions", false)]
	public Task<Profile?> LookupProfileWithRelation(ProfileId localId, Uri relationId);

	public IAsyncEnumerable<Profile> FindProfilesByHandle(string handle, bool partial = false, int limit = 20, int page = 0);

	public void Add(Profile profile);
	public void Add(Account account);
	public void Add(Post post);
	public void AddRange(IEnumerable<Profile> profile);
	public void AddRange(IEnumerable<Account> account);
	public void Update(Profile profile);
	public void Update(Account account);
	public void Update(Post post);
	public void Update(Audience audience);
	public void UpdateRange(IEnumerable<Profile> profile);
	public void UpdateRange(IEnumerable<Account> account);
	public void Delete(object record);
	public void Remove(Post post);
	public void Remove(Content post);

	public Task Cancel();
	public Task Commit();
}