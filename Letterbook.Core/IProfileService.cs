using System.Security.Claims;
using Letterbook.Core.Models;
using Letterbook.Core.Values;
using Medo;

namespace Letterbook.Core;

public interface IProfileService
{
	IAuthzProfileService As(IEnumerable<Claim> claims);
}

public interface IAuthzProfileService
{
	Task<Profile> CreateProfile(Profile profile);
	Task<Profile> CreateProfile(Uuid7 ownerId, string handle);

	/// <summary>
	/// Set a new display name for the given profile
	/// </summary>
	/// <param name="localId"></param>
	/// <param name="displayName"></param>
	/// <returns>The original and updated Profiles, or null if no change was made</returns>
	/// <exception cref="CoreException"></exception>
	Task<UpdateResponse<Profile>> UpdateDisplayName(Uuid7 localId, string displayName);
	Task<UpdateResponse<Profile>> UpdateDescription(Uuid7 localId, string description);
	Task<UpdateResponse<Profile>> InsertCustomField(Uuid7 localId, int index, string key, string value);
	Task<UpdateResponse<Profile>> RemoveCustomField(Uuid7 localId, int index);
	Task<UpdateResponse<Profile>> UpdateCustomField(Uuid7 localId, int index, string key, string value);
	Task<UpdateResponse<Profile>> UpdateProfile(Profile profile);

	/// <summary>
	/// Lookup a profile, optionally including relationship information to another profile
	/// </summary>
	/// <remarks>
	/// Should require `Follow` and `ApproveFollower` on the selfProfile, in order to check existing relationships
	/// </remarks>
	/// <param name="profileId">The profile to lookup</param>
	/// <param name="relatedProfile">The profile to check for a relationship</param>
	/// <returns></returns>
	Task<Profile?> LookupProfile(Uuid7 profileId, Uuid7? relatedProfile = null);

	/// <see cref="LookupProfile(Medo.Uuid7,System.Nullable{Medo.Uuid7})"/>
	Task<Profile?> LookupProfile(Uri fediId, Uuid7? relatedProfile = null);
	Task<IEnumerable<Profile>> FindProfiles(string handle);

	/// <see cref="Follow(Medo.Uuid7,Medo.Uuid7)"/>
	Task<FollowerRelation> Follow(Uuid7 selfId, Uri targetId);

	/// <summary>
	/// Request to follow the target profile
	/// </summary>
	/// <remarks>Returned relationship state is likely to be transient for target profiles hosted on remote servers</remarks>
	/// <param name="selfId"></param>
	/// <param name="targetId"></param>
	/// <returns></returns>
	Task<FollowerRelation> Follow(Uuid7 selfId, Uuid7 targetId);
	Task<FollowerRelation> ReceiveFollowRequest(Uri targetId, Uri followerId, Uri? requestId);
	Task<FollowerRelation> ReceiveFollowRequest(Uuid7 localId, Uri followerId, Uri? requestId);

	/// <summary>
	/// Update the follow relationship with the replied state
	/// </summary>
	/// <param name="selfId"></param>
	/// <param name="targetId"></param>
	/// <param name="response"></param>
	/// <returns></returns>
	Task<FollowerRelation> ReceiveFollowReply(Uuid7 selfId, Uri targetId, FollowState response);

	/// <summary>
	/// Remove the target profile as a follower
	/// </summary>
	/// <remarks>This will also reject a pending follow request from the follower</remarks>
	/// <param name="selfId"></param>
	/// <param name="followerId"></param>
	/// <returns></returns>
	Task<FollowerRelation> RemoveFollower(Uuid7 selfId, Uri followerId);

	/// <see cref="RemoveFollower(Medo.Uuid7,System.Uri)"/>
	Task<FollowerRelation> RemoveFollower(Uuid7 selfId, Uuid7 followerId);

	/// <summary>
	/// Stop following the target
	/// </summary>
	/// <param name="selfId"></param>
	/// <param name="targetId"></param>
	/// <returns></returns>
	Task<FollowerRelation> Unfollow(Uuid7 selfId, Uri targetId);

	/// <see cref="Unfollow(Medo.Uuid7,System.Uri)"/>
	Task<FollowerRelation?> Unfollow(Uuid7 selfId, Uuid7 targetId);

	/// <summary>
	/// Lookup the list of profiles that the specified profile is following
	/// </summary>
	/// <remarks>Results are ordered reverse chronologically, by the date the relation began (most recent first)</remarks>
	/// <param name="profileId"></param>
	/// <param name="followedBefore">Get relations that began before this date</param>
	/// <param name="limit"></param>
	/// <returns></returns>
	Task<IQueryable<Profile>> LookupFollowing(Uuid7 profileId, DateTimeOffset? followedBefore, int limit);

	/// <summary>
	/// Lookup the list of profiles that follow the specified profile
	/// </summary>
	/// <remarks>Results are ordered reverse chronologically, by the date the relation began (most recent first)</remarks>
	/// <param name="profileId"></param>
	/// <param name="followedBefore">Get relations that began before this date</param>
	/// <param name="limit"></param>
	/// <returns></returns>
	Task<IQueryable<Profile>> LookupFollowers(Uuid7 profileId, DateTimeOffset? followedBefore, int limit);

	/// <summary>
	/// Approve a follow request from the target profile
	/// </summary>
	/// <param name="profileId"></param>
	/// <param name="followerId"></param>
	/// <returns></returns>
	Task<FollowerRelation> AcceptFollower(Uuid7 profileId, Uuid7 followerId);

	/// <see cref="AcceptFollower(Medo.Uuid7,Medo.Uuid7)"/>
	Task<FollowerRelation> AcceptFollower(Uuid7 profileId, Uri followerId);

	Task<int> FollowerCount(Profile profile);
	Task<int> FollowingCount(Profile profile);

	Task ReportProfile(Uuid7 selfId, Uri profileId);


	// - [ ] receive report


	// - [ ] block


	// - [ ] mute


	// - [ ] subscribe (follow, but only see public posts)


	// - [ ] transfer in


	// - [ ] transfer out


	// - [ ] delete profile


	// - [ ] grant access to another account


	// - [ ] revoke access
}
