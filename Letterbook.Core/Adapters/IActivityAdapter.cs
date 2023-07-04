using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Adapters;

public interface IActivityAdapter
{
    Task RecordObject(Object obj);
}