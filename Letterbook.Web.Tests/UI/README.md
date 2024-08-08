# E2E Testing for Letterbook.Web

## Concepts

There are two concepts here, and we'd like to allow them to vary separately so that the tests don't have to change.

Each concept is represented by an independent fixture.

### PlaywrightFixture

This just gives a browser instance. All it needs to be able to run is a URL.

### WebServerFixture [WIP]

`WebServerFixture` Can be used to optionally start a local version of the server. You can choose to use this or not.

At the moment it will only be used if the port specified in `Settings.BaseUrl` is not already in use.

This is useful now because I am having trouble getting `WebServerFixture` to work completely.

Allowing switching like this means we can use the tests to help verify it.

There may be benefits to this approach or we may not be able to make it work at all, not yet sure.

Meanwhile, though, the tests can be targeted at any other web server.

## Tests depend on UI only

```c#
public class LandingPageValidationTest(ITestOutputHelper log, PlaywrightFixture playwrightFixture)
	: IClassFixture<PlaywrightFixture>, IClassFixture<WebServerFixture>
{
	[Fact]
	public async Task HomepageHasCorrectTitleAndLinksToAdminProfile()
	{
		var page = await playwrightFixture.Browser!.NewPageAsync();

		await page.GotoAsync($"{Settings.BaseUrl}");

		Assert.Matches("Letterbook.Web", await page.TitleAsync());

		var adminProfileLink = page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Admin profile" });

		var href = await adminProfileLink.GetAttributeAsync("href");

		Assert.Equal("/@admin", href);
	}
}
```

### Examples

#### [DRAFT, WIP] Run with `WebServerFixture`

By default, running the following does so against a newly-started server on a random port:

```shell
dotnet test Letterbook.Web.Tests/Letterbook.Web.Tests.csproj --filter DisplayName~LandingPageValidationTest
```

You'll see some extra console output showing that the server has started:

```shell
[11:23:40 INF] Starting server at <http://localhost:39395/>
[11:23:40 INF] CurrentDirectory: /home/ben/sauce/Letterbook/Letterbook.Web.Tests/bin/Debug/net8.0
[11:23:40 INF] ContentRootPath: /home/ben/sauce/Letterbook/Letterbook.Web
[11:23:40 INF] RazorPagesRootDirectory: /home/ben/sauce/Letterbook/Letterbook.Web/Pages
[11:23:42 INF] Configured endpoint delivery-worker, Consumer: Letterbook.Workers.Consumers.DeliveryWorker
[11:23:42 INF] Configured endpoint timeline, Consumer: Letterbook.Workers.Consumers.TimelineConsumer
[11:23:42 INF] Bus started: loopback://localhost/
[11:23:45 DBG] Found accounts, skipping seed
[11:23:45 INF] Application started
[11:23:45 INF] Listening on http://localhost:39395
```

You'll also notice that `Letterbook.Web.Tests.UI.LandingPageValidationTest.HomepageHasCorrectTitleAndLinksToAdminProfile` is skipped.

That's because [I can't make it pass yet](./LandingPageValidationTest.cs).

#### Run against already-running server

```shell
BaseUrl=https://localhost:5127 dotnet test Letterbook.Web.Tests/Letterbook.Web.Tests.csproj --filter DisplayName~LandingPageValidationTest
```

#### Run with headed browser

```shell
Headless=false dotnet test Letterbook.Web.Tests/Letterbook.Web.Tests.csproj --filter DisplayName~LandingPageValidationTest
```

## Setup

This test package uses [Playwright](https://playwright.dev). While most dependencies are provided automatically for you, Playwright will require you to install test browsers to run these tests.

You can find out more about this at https://playwright.dev/dotnet/docs/intro

Or use

```shell
dotnet build
pwsh bin/Debug/netX/playwright.ps1 install
```

To install browser dependencies. This requires powershell to be installed.

> Note: You need to replace `netX` above with the version of .NET you are using. For example `pwsh .\bin\Debug\net7.0\playwright.ps1 install`

## Common Errors

### Executable doesn't exist at **/chrome.exe | **/firefox.exe

Playwright uses custom headless versions of common browsers to execute its tests. If your seeing an error that chrome/firefox etc can not be found this does not mean you need to reinstall your browsers. Instead you'll need to run the configuration script provided by Playwright to install the necessary browsers for the test clients to run.

Playwright should have provided a hint to install the needed browser tooling or you can use the Setup steps above.

### The argument 'bin/Debug/netX/playwright.ps1' is not recognized as the name of a script file.

`netX` is a placeholder for the version of .NET you are using. You should replace that with the correct version and try again.

### Npgsql.NpgsqlException : Failed to connect to 127.0.0.1:5432

When running on the build server this is because it expecting a database to exist first.
