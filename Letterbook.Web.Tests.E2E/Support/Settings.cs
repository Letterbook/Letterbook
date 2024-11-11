using System.Net;
using System.Net.Sockets;

namespace Letterbook.Web.Tests.E2E.Support;

static class Settings
{
	public const int DefaultPort = 5127;
	public static Uri BaseUrl = new(Get(nameof(BaseUrl), $"http://localhost:{DefaultPort}"));

	private static string Get(string name, string @default)
		=> Environment.GetEnvironmentVariable(name) ?? @default;
}