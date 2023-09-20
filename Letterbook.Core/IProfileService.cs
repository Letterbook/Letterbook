using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core;

public interface IProfileService
{
    Task<Profile> CreateProfile(Profile profile);
    Task<Profile> CreateProfile(Guid ownerId, string handle);
    Task<UpdateResponse<Profile>> UpdateDisplayName(Guid localId, string displayName);
    Task<UpdateResponse<Profile>> UpdateDescription(Guid localId, string description);
    Task<UpdateResponse<Profile>> InsertCustomField(Guid localId, int index, string key, string value);
    Task<UpdateResponse<Profile>> RemoveCustomField(Guid localId, int index);
    Task<UpdateResponse<Profile>> UpdateCustomField(Guid localId, int index, string key, string value);
    Task<UpdateResponse<Profile>> UpdateProfile(Profile profile);
    Task<Profile?> LookupProfile(Guid localId);
    Task<Profile?> LookupProfile(Uri id);
    Task<IEnumerable<Profile>> FindProfiles(string handle);
    Task<FollowResult> Follow(Guid selfId, Uri profileId, Uri? audienceId);
    Task<FollowResult> Follow(Guid selfId, Guid localId, Uri? audienceId);
    Task<FollowResult> ReceiveFollowRequest(Uri selfId, Uri followerId);
    Task RemoveFollower(Guid selfId, Uri followerId);
    Task RemoveFollower(Guid selfId, Guid followerId);
    Task Unfollow(Guid selfId, Uri followerId);
    Task Unfollow(Guid selfId, Guid followerId);
    Task ReportProfile(Guid selfId, Uri profileId);
    Task ReportProfile(Guid selfId, Guid localId);
    
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