using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core;

public interface IProfileService
{
    Task<Profile> CreateProfile(Profile profile);
    Task<Profile?> CreateProfile(Guid ownerId, string handle);
    Task<Profile> UpdateProfile(Profile profile);
    Task<Profile> LookupProfile(Guid localId);
    Task<Profile> LookupProfile(Uri id);
    Task<IEnumerable<Profile>> FindProfiles(string handle);
    Task<FollowResult> Follow(Guid selfId, Uri profileId, Uri? audienceId);
    Task<FollowResult> Follow(Guid selfId, Guid localId, Uri? audienceId);
    Task<FollowResult> ReceiveFollower(Uri selfId, Uri followerId);
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