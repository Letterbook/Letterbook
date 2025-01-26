using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Core.Queries;

public static class PostQueries
{
	public static IQueryable<Post> WithThread(this IQueryable<Post> query) =>
		query.Include(post => post.Thread)
			.AsSplitQuery();

	/// <summary>
	/// Include data from navigations that are frequently necessary to process a Post or make authz decisions
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<Post> WithCommonData(this IQueryable<Post> query) =>
		query.Include(p => p.Creators)
			.Include(p => p.Audience)
			.Include(p => p.Contents)
			.AsSplitQuery();
}