using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Adapters;

public interface IFediAdapter : IAdapter
{
    Task<PubObject> FollowLink(Uri href);
}