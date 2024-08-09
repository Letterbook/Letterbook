using System.Net;
using System.Net.Sockets;

namespace Letterbook.Web.Tests.E2E.Support;

static class Settings
{
	public const int DefaultPort = 5127;
	public static Uri BaseUrl = new(Get(nameof(BaseUrl), $"http://localhost:{Port()}"));
	public static bool Headless => bool.Parse(Get(nameof(Headless), bool.TrueString));
	public static bool NoSkip => bool.Parse(Get(nameof(NoSkip), bool.FalseString));
	public static bool AllowWebServerFixture => bool.Parse(Get(nameof(AllowWebServerFixture), bool.TrueString));

	private static string Get(string name, string @default)
		=> Environment.GetEnvironmentVariable(name) ?? @default;

	private static int Port() => AllowWebServerFixture ? GetRandomUnusedPort(): DefaultPort;

	private static int GetRandomUnusedPort()
	{
		using var listener = new TcpListener(IPAddress.Any, 0);

		listener.Start();
		var port = ((IPEndPoint)listener.LocalEndpoint).Port;
		listener.Stop();

		return port;
	}
}