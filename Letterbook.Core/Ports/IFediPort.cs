using PubObject = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Ports;

public interface IFediPort
{
    Task<PubObject> FollowLink(Uri href);
}