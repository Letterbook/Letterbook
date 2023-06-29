using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Ports;

public interface ISharePort
{
    Task ShareWithAudience(Object obj, string audienceUri);
}