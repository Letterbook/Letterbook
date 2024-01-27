using Letterbook.Core.Models;
using Letterbook.Core.Values;

namespace Letterbook.Core.Adapters;

public interface IActivityPubClient
{
    IActivityPubAuthenticatedClient As(Profile? onBehalfOf);
    Task<T> Fetch<T>(Uri id) where T : IFederated;
}