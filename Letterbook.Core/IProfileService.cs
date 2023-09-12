using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core;

public interface IProfileService
{
    // - [ ] create profile
    Task<Profile> CreateProfile(Profile profile);
    // - [ ] modify profile
    Task<Profile> UpdateProfile(Profile profile);
    // - [ ] lookup profile
    Task<Profile> LookupProfile(Guid localId);
    Task<Profile> LookupProfile(Uri id);
    // - [ ] search for profiles
    // Task<IEnumerable<Profile>> FindProfiles();
    // - [ ] follow someone
    Task<FollowResult> Follow(Guid selfId, Uri profileId, Uri? audienceId);
    Task<FollowResult> Follow(Guid selfId, Guid localId, Uri? audienceId);
    // - [ ] be followed by someone
    Task<FollowResult> ReceiveFollower(Uri selfId, Uri followerId);
    // - [ ] remove follower
    Task RemoveFollower(Guid selfId, Uri followerId);
    Task RemoveFollower(Guid selfId, Guid followerId);
    // - [ ] unfollow
    Task Unfollow(Guid selfId, Uri followerId);
    Task Unfollow(Guid selfId, Guid followerId);
    // - [ ] send report
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