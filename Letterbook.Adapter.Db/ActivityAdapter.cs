using Letterbook.Core.Ports;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Adapter.Db;

public class ActivityAdapter : IActivityPort
{
    public Task RecordObject(Object obj)
    {
        throw new NotImplementedException();
    }
}