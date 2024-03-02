namespace Letterbook.Core.Extensions;

public static class UriExtensions
{
    public static string GetAuthority(this Uri uri) => uri.IsDefaultPort
        ? string.Join('.', uri.Host.Split('.').Reverse())
        : string.Join('.', uri.Host.Split('.').Reverse()) + uri.Port;
}