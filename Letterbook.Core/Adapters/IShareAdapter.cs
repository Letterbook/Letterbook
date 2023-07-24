using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IShareAdapter : IAdapter
{
    Task ShareWithAudience(IObjectRef obj, string audienceUri);
}