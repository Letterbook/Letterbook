using Letterbook.Core.Models;
using Medo;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Core.Extensions;

public static class QueryExtensions
{
	public static IQueryable<Profile> WithRelation(this IQueryable<Profile> query, Uri relationId) =>
		query.Include(profile => profile.FollowingCollection.Where(relation => relation.Follows.FediId == relationId))
			.ThenInclude(relation => relation.Follows)
			.Include(profile => profile.FollowersCollection.Where(relation => relation.Follower.FediId == relationId))
			.ThenInclude(relation => relation.Follower)
			.Include(profile => profile.Keys)
			.Include(profile => profile.Audiences)
			.AsSplitQuery();

	public static IQueryable<Profile> WithRelation(this IQueryable<Profile> query, Uuid7 relationId) =>
		query.Include(profile => profile.FollowingCollection.Where(relation => relation.Follows.Id == relationId.ToGuid()))
			.ThenInclude(relation => relation.Follows)
			.Include(profile => profile.FollowersCollection.Where(relation => relation.Follower.Id == relationId.ToGuid()))
			.ThenInclude(relation => relation.Follower)
			.Include(profile => profile.Keys)
			.Include(profile => profile.Audiences)
			.AsSplitQuery();
}