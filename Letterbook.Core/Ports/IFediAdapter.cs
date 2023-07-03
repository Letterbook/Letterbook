using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Ports;

public interface IFediAdapter
{
    Task<PubObject> FollowLink(Uri href);
}