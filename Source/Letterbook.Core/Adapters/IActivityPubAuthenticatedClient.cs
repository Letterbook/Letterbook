using ActivityPub.Types.AS;
using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core.Adapters;

public interface IActivityPubAuthenticatedClient : IActivityPubClient
{
	Task<ClientResponse<object>> SendDocument(Uri inbox, ASType document);
	Task<ClientResponse<object>> SendDocument(Uri inbox, string document);
}