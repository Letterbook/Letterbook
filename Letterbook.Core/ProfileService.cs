using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core;

class ProfileService : IProfileService
{
    public Task<Profile> CreateProfile(Profile profile)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> UpdateProfile(Profile profile)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> LookupProfile(Guid localId)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> LookupProfile(Uri id)
    {
        throw new NotImplementedException();
    }

    public Task<FollowResult> Follow(Guid selfId, Uri profileId, Uri? audienceId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowResult> Follow(Guid selfId, Guid localId, Uri? audienceId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowResult> ReceiveFollower(Uri selfId, Uri followerId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFollower(Guid selfId, Uri followerId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFollower(Guid selfId, Guid followerId)
    {
        throw new NotImplementedException();
    }

    public Task Unfollow(Guid selfId, Uri followerId)
    {
        throw new NotImplementedException();
    }

    public Task Unfollow(Guid selfId, Guid followerId)
    {
        throw new NotImplementedException();
    }

    public Task ReportProfile(Guid selfId, Uri profileId)
    {
        throw new NotImplementedException();
    }

    public Task ReportProfile(Guid selfId, Guid localId)
    {
        throw new NotImplementedException();
    }
}