using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core.Adapters;

public interface IActivityPubClient
{
    IActivityPubClient As(Profile? onBehalfOf);
    Task<FollowState> SendFollow(Uri inbox, FollowerRelation request);
    Task<object> SendCreate(Uri inbox, IContentRef content);
    Task<object> SendUpdate(Uri inbox, IContentRef content);
    Task<object> SendDelete(Uri inbox, IContentRef content);
    Task<object> SendBlock(Uri inbox, IContentRef content);
    Task<object> SendBoost(Uri inbox, IContentRef content);
    Task<object> SendLike(Uri inbox, IContentRef content);
    Task<object> SendDislike(Uri inbox, IContentRef content);
    Task<object> SendAccept(Uri inbox, IContentRef content);
    Task<object> SendReject(Uri inbox, IContentRef content);
    Task<object> SendPending(Uri inbox, IContentRef content);
    Task<object> SendAdd(Uri inbox, IContentRef content, Uri collection);
    Task<object> SendRemove(Uri inbox, IContentRef content, Uri collection);
    Task<object> SendUnfollow(Uri inbox);
    Task<T> Fetch<T>(Uri id) where T : IObjectRef;
}