using System.Net;
using System.Net.Sockets;

namespace Letterbook.Web.Tests.UI.Support;

static class Settings
{
	public const int DefaultPort = 5127;
	public static Uri BaseUrl = new(Get(nameof(BaseUrl), $"http://localhost:{GetRandomUnusedPort()}"));
	public static bool Headless => bool.Parse(Get(nameof(Headless), bool.TrueString));
	public static bool NoSkip => bool.Parse(Get(nameof(NoSkip), bool.FalseString));

	private static string Get(string name, string @default)
		=> Environment.GetEnvironmentVariable(name) ?? @default;

	private static int GetRandomUnusedPort()
	{
		using var listener = new TcpListener(IPAddress.Any, 0);

		listener.Start();
		var port = ((IPEndPoint)listener.LocalEndpoint).Port;
		listener.Stop();

		return port;
	}
}