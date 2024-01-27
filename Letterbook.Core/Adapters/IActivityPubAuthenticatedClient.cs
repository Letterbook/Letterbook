using ActivityPub.Types.AS;
using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core.Adapters;

public interface IActivityPubAuthenticatedClient : IActivityPubClient
{
    Task<ClientResponse<FollowState>> SendFollow(Uri inbox, Profile profile);
    Task<ClientResponse<object>> SendCreate(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendUpdate(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendDelete(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendBlock(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendBoost(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendLike(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendDislike(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendAccept(Uri inbox, ActivityType activityToAccept, Uri requestorId, Uri? subjectId);
    Task<ClientResponse<object>> SendReject(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendPending(Uri inbox, IFederated content);
    Task<ClientResponse<object>> SendAdd(Uri inbox, IFederated content, Uri collection);
    Task<ClientResponse<object>> SendRemove(Uri inbox, IFederated content, Uri collection);
    Task<ClientResponse<object>> SendUnfollow(Uri inbox);
    Task<ClientResponse<object>> SendDocument(Uri inbox, ASType document);
    Task<ClientResponse<object>> SendDocument(Uri inbox, string document);
}