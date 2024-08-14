namespace Letterbook.Web.Tests.E2E.Support;

public static class Playwright
{
	/*
		Equivalent to:

			pwsh Letterbook.Web.Tests/bin/Release/net8.0/playwright.ps1 install firefox

		See: https://playwright.dev/dotnet/docs/browsers#managing-browser-binaries for where binaries go to.
		See: https://playwright.dev/dotnet/docs/browsers#install-browsers for more options
	*/
	public static void Install()
	{
		var exitCode = Microsoft.Playwright.Program.Main(new [] { "install", "firefox" });

		if (exitCode != 0)
		{
			throw new Exception($"Playwright exited with code {exitCode}");
		}
	}
}