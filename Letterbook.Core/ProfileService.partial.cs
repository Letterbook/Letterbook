using Letterbook.Core.Values;

namespace Letterbook.Core;

public partial class ProfileService
{
    public Task<FollowState> Follow(Guid selfId, Uri targetId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowState> Follow(Guid selfId, Guid localId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowState> ReceiveFollowRequest(Uri targetId, Uri followerId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowState> ReceiveFollowRequest(Guid localId, Uri followerId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowState> ReceiveFollowReply(Uri selfId, Uri targetId, FollowState response)
    {
        throw new NotImplementedException();
    }
}