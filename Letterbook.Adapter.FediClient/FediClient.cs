using Letterbook.Core.Adapters;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.FediClient;

public class FediClient : IFediAdapter
{
    public Task<IObjectRef> FollowLink(Uri href)
    {
        throw new NotImplementedException();
    }
}