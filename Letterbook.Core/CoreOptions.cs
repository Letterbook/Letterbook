namespace Letterbook.Core;

public class CoreOptions
{
	public const string ConfigKey = "Letterbook";

	public string DomainName { get; set; } = "letterbook.example";
	public string Scheme { get; set; } = "https";
	public string Port { get; set; } = "443";
	public int MaxCustomFields { get; set; } = 10;

	public DatabaseOptions Database { get; set; } = new();
}

public class DatabaseOptions
{
	public bool MigrateAtRuntime { get; set; } = false;
}