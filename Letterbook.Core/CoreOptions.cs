namespace Letterbook.Core;

public class CoreOptions
{
    public const string ConfigKey = "Letterbook";

    public string DomainName { get; set; } = "letterbook.example";
    public string Scheme { get; set; } = "https";
    public string Port { get; set; } = "443";
}