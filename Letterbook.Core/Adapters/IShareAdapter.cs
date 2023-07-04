using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Core.Adapters;

public interface IShareAdapter
{
    Task ShareWithAudience(Object obj, string audienceUri);
}