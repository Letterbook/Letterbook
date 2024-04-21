using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityPubClient
{
	IActivityPubAuthenticatedClient As(IFederatedActor? onBehalfOf);
	Task<T> Fetch<T>(Uri id) where T : IFederated;
	Task<T> Fetch<T>(Uri id, SigningKey actorSigningKey) where T : IFederated;
}