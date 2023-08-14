using Letterbook.Core.Models;

namespace Letterbook.Core.Extensions;

public static class CoreOptionsExtensions
{
    public static bool HasLocalAuthority(this IObjectRef subject, CoreOptions core)
    {
        return core.BaseUri().Authority == subject.Authority;
    }
    
    public static Uri BaseUri(this CoreOptions coreOptions)
    {
        return new Uri($"{coreOptions.Scheme}://{coreOptions.DomainName}:{coreOptions.Port}");
    }
}