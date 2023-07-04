using Letterbook.Core.Adapters;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Adapter.FediClient;

public class FediClient : IFediAdapter
{
    public Task<Object> FollowLink(Uri href)
    {
        throw new NotImplementedException();
    }
}