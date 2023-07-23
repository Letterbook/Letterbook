using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IShareAdapter
{
    Task ShareWithAudience(IObjectRef obj, string audienceUri);
}