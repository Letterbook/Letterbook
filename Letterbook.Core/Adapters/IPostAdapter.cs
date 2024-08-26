using System.Linq.Expressions;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core.Adapters;

public interface IPostAdapter
{
	public Task<Post?> LookupPost(Uuid7 postId);
	public Task<Post?> LookupPost(Uri fediId);
	public Task<ThreadContext?> LookupThread(Uri threadId);
	public Task<ThreadContext?> LookupThread(Uuid7 threadId);
	public Task<Post?> LookupPostWithThread(Uuid7 postId);
	public Task<Post?> LookupPostWithThread(Uri postId);
	public Task<Profile?> LookupProfile(Uuid7 profileId);
	public Task<Profile?> LookupProfile(Uri profileId);
	public void Add(Post post);
	public void Add(Profile profile);
	public void AddRange(IEnumerable<Post> posts);
	public void Update(Post post);
	public void Update(Profile profile);
	public void UpdateRange(IEnumerable<Post> post);
	public void Remove(Post post);
	public void Remove(Content content);
	public void RemoveRange(IEnumerable<Post> posts);
	public void RemoveRange(IEnumerable<Content> contents);

	/// <summary>
	/// Query for navigation entities, using the given post as a root entity
	/// </summary>
	/// <remarks>Consumers should be sure to set an OrderBy property</remarks>
	/// <param name="post"></param>
	/// <param name="queryExpression">An expression func that specifies the navigation property to query</param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public IQueryable<T> QueryFrom<T>(Post post, Expression<Func<Post, IEnumerable<T>>> queryExpression)
		where T : class;

	public Task Cancel();
	public Task Commit();
}