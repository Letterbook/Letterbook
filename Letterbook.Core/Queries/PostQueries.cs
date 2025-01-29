using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Core.Queries;

public static class PostQueries
{
	/// <summary>
	/// Include the Thread and other Posts
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<Post> WithThread(this IQueryable<Post> query) =>
		query.Include(post => post.Thread).ThenInclude(thread => thread.Posts).ThenInclude(p => p.Creators)
			.Include(post => post.Thread).ThenInclude(thread => thread.Posts).ThenInclude(p => p.Contents)
			.AsSplitQuery();

	/// <summary>
	/// Include data from navigations that are frequently necessary to process a Post or make authz decisions
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<Post> WithCommonFields(this IQueryable<Post> query) =>
		query.Include(p => p.Creators)
			.Include(p => p.Audience)
			.Include(p => p.Contents)
			.AsSplitQuery();
}