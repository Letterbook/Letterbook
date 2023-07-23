using Letterbook.Core.Adapters;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.FediClient;

[AutoAdapter(typeof(IFediAdapter))]
public class FediClient : IFediAdapter
{
    public Task<IObjectRef> FollowLink(Uri href)
    {
        throw new NotImplementedException();
    }
}