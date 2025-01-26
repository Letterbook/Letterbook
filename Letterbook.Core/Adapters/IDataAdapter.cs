using System.Linq.Expressions;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IDataAdapter : IDisposable
{
	delegate IQueryable<Profile> ProfileQuery(IQueryable<Profile> querySource);

	delegate IQueryable<Account> AccountQuery(IQueryable<Account> querySource);

	delegate bool ProfileComparer(Profile profile);

	public bool RecordAccount(Account account);
	public Task<Account?> LookupAccount(Guid id);
	public Task<Account?> FindAccountByEmail(string normalizedEmail);

	/// <summary>
	/// Query across all Accounts
	/// </summary>
	/// <returns></returns>
	public IQueryable<Account> AllAccounts();

	/// <summary>
	/// Query for Posts by ID
	/// </summary>
	/// <param name="postIds"></param>
	/// <returns></returns>
	public IQueryable<Post> Posts(params PostId[] postIds);

	/// <see cref="Posts(Letterbook.Core.Models.PostId[])"/>
	public IQueryable<Post> Posts(params Uri[] postIds);

	/// <summary>
	/// Query for a Thread by Id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public IQueryable<ThreadContext> Threads(ThreadId id);
	/// <see cref="Threads(Medo.Uuid7)"/>
	public IQueryable<ThreadContext> Threads(params Uri[] threadIds);

	/// <summary>
	/// Query for Profiles by ID
	/// </summary>
	/// <param name="ids"></param>
	/// <returns></returns>
	public IQueryable<Profile> Profiles(params ProfileId[] ids);

	/// <see cref="Profiles(Letterbook.Core.Models.ProfileId[])"/>
	public IQueryable<Profile> Profiles(params Uri[] ids);

	/// <summary>
	/// Query across all Profiles
	/// </summary>
	/// <returns></returns>
	public IQueryable<Profile> AllProfiles();

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

	/// <see cref="QueryFrom{T}(Letterbook.Core.Models.Profile,System.Linq.Expressions.Expression{System.Func{Letterbook.Core.Models.Profile,System.Collections.Generic.IEnumerable{T}}})"/>
	public IQueryable<T> QueryFrom<T>(Post post, Expression<Func<Post, IEnumerable<T>>> queryExpression)
		where T : class;

	/// <summary>
	/// Query across all Audiences
	/// </summary>
	/// <returns></returns>
	public IQueryable<Audience> AllAudience();

	public void Add(Profile profile);
	public void Add(Account account);
	public void Add(Post post);
	public void AddRange(IEnumerable<Profile> profile);
	public void AddRange(IEnumerable<Account> account);
	public void AddRange(IEnumerable<Post> posts);
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