namespace Letterbook.Core.Exceptions;

public class ConfigException : Exception
{
	private ConfigException(string? message) : base(message) { }

	public static ConfigException Missing(string section) =>
		new($"Missing required config ({section}). Did you provide an appsettings file?");
}