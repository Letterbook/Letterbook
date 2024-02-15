using Letterbook.Core.Models;

namespace Letterbook.Core.Extensions;

public static class CoreOptionsExtensions
{
    public static bool HasLocalAuthority(this Uri id, CoreOptions core) =>
        core.BaseUri().Authority == id.Authority;
    
    public static bool HasLocalAuthority(this IFederated subject, CoreOptions core) =>
        core.BaseUri().Authority == subject.Authority;

    public static bool HasLocalAuthority(this CoreOptions coreOptions, string domain) =>
        coreOptions.BaseUri().Authority == domain;

    public static bool HasLocalAuthority(this CoreOptions coreOptions, Uri uri) =>
        coreOptions.BaseUri().Authority == uri.Authority;

    public static Uri BaseUri(this CoreOptions coreOptions) =>
        new($"{coreOptions.Scheme}://{coreOptions.DomainName}:{coreOptions.Port}");
}