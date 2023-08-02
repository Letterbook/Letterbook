namespace Letterbook.Core;

public class CoreOptions
{
    public const string ConfigKey = "Letterbook";

    public string DomainName { get; set; } = "letterbook.example";
    public string Scheme { get; set; } = "https";
    public string Port { get; set; } = "443";

    public static Uri BaseUri(CoreOptions coreOptions)
    {
        return new Uri($"{coreOptions.Scheme}://{coreOptions.DomainName}:{coreOptions.Port}");
    }
}