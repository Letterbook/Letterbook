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
			.Include(profile => profile.Audiences.Where(audience => audience.Source != null && audience.Source.FediId == relationId))
			.ThenInclude(audience => audience.Source)
			.Include(profile => profile.Headlining)
			.ThenInclude(audience => audience.Members.Where(member => member.FediId == relationId))
			.Include(profile => profile.Headlining)
			.AsSplitQuery();

	public static IQueryable<Profile> WithRelation(this IQueryable<Profile> query, ProfileId? relationId) =>
		query.Include(profile => profile.FollowingCollection.Where(relation => relation.Follows.Id == relationId))
			.ThenInclude(relation => relation.Follows)
			.Include(profile => profile.FollowersCollection.Where(relation => relation.Follower.Id == relationId))
			.ThenInclude(relation => relation.Follower)
			.Include(profile => profile.Keys)
			.Include(profile => profile.Audiences.Where(audience => audience.Source != null && audience.Source.Id == relationId))
			.ThenInclude(audience => audience.Source)
			.Include(profile => profile.Headlining)
			.ThenInclude(audience => audience.Members.Where(member => member.Id == relationId))
			.AsSplitQuery();

	public static IQueryable<Profile> WithKeys(this IQueryable<Profile> query) =>
		query.Include(profile => profile.Keys)
			.AsSplitQuery();
}