using Fedodo.NuGet.ActivityPub.Model.CoreTypes;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Ports;

public interface IActivityAdapter
{
    Task RecordObject(Object obj);
}