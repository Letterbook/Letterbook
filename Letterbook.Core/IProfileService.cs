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
	Task<Profile> CreateProfile(Guid ownerId, string handle);
	Task<UpdateResponse<Profile>> UpdateDisplayName(Guid localId, string displayName);
	Task<UpdateResponse<Profile>> UpdateDescription(Guid localId, string description);
	Task<UpdateResponse<Profile>> InsertCustomField(Guid localId, int index, string key, string value);
	Task<UpdateResponse<Profile>> RemoveCustomField(Guid localId, int index);
	Task<UpdateResponse<Profile>> UpdateCustomField(Guid localId, int index, string key, string value);
	Task<UpdateResponse<Profile>> UpdateProfile(Profile profile);
	Task<Profile?> LookupProfile(Uuid7 localId);
	Task<Profile?> LookupProfile(Uri id);
	Task<IEnumerable<Profile>> FindProfiles(string handle);
	Task<FollowState> Follow(Guid selfId, Uri targetId);
	Task<FollowState> Follow(Guid selfId, Guid localId);
	Task<FollowerRelation> ReceiveFollowRequest(Uri targetId, Uri followerId, Uri? requestId);
	Task<FollowerRelation> ReceiveFollowRequest(Guid localId, Uri followerId, Uri? requestId);
	Task<FollowState> ReceiveFollowReply(Uri selfId, Uri targetId, FollowState response);
	Task RemoveFollower(Guid selfId, Uri followerId);
	Task Unfollow(Guid selfId, Uri followerId);
	Task ReportProfile(Guid selfId, Uri profileId);

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