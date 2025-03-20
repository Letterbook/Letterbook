using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Core.Queries;

public static class ProfileQueries
{
	public static IQueryable<Profile> WithRelation(this IQueryable<Profile> query, Uri? relationId) =>
		query.Include(profile => profile.FollowingCollection.Where(relation => relation.Follows.FediId == relationId))
			.ThenInclude(relation => relation.Follows)
			.Include(profile => profile.FollowersCollection.Where(relation => relation.Follower.FediId == relationId))
			.ThenInclude(relation => relation.Follower)
			.Include(profile => profile.Keys)
			.Include(profile => profile.Audiences)
			.AsSplitQuery();

	public static IQueryable<Profile> WithRelation(this IQueryable<Profile> query, ProfileId? relationId) =>
		query.Include(profile => profile.FollowingCollection.Where(relation => relation.Follows.Id == relationId))
			.ThenInclude(relation => relation.Follows)
			.Include(profile => profile.FollowersCollection.Where(relation => relation.Follower.Id == relationId))
			.ThenInclude(relation => relation.Follower)
			.Include(profile => profile.Keys)
			.Include(profile => profile.Audiences)
			.AsSplitQuery();

	public static IQueryable<Profile> WithKeys(this IQueryable<Profile> query) =>
		query.Include(profile => profile.Keys)
			.AsSplitQuery();
}