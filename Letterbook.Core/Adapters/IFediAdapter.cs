using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Adapters;

public interface IFediAdapter
{
    Task<PubObject> FollowLink(Uri href);
}