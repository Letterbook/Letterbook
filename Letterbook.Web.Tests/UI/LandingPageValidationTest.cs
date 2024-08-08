using Letterbook.Web.Tests.UI.Support;
using Microsoft.Playwright;

namespace Letterbook.Web.Tests.UI;

// dotnet test Letterbook.Web.Tests/Letterbook.Web.Tests.csproj --filter DisplayName~LandingPageValidationTest
public class LandingPageValidationTest(PlaywrightFixture playwrightFixture) : IClassFixture<PlaywrightFixture>, IClassFixture<WebServerFixture>
{
	/*

		Useful at the moment because when running against `WebServerFixture` this works but razor pages do not.

		So it is a sanity check to show that the server is running and serving static files.

	 */
	[Fact]
	public async Task HomepageHasAFavicon()
	{
		var page = await playwrightFixture.Browser!.NewPageAsync();

		await page.GotoAsync($"{Settings.BaseUrl}favicon.ico");
	}

	/*

		Help wanted.

		I can so far *only* get this to pass when running against the default development server:

			BaseUrl=http://localhost:5127 dotnet test Letterbook.Web.Tests/Letterbook.Web.Tests.csproj --filter DisplayName~LandingPageValidationTest

		There seems to be a problem with razor pages when running with `WebServerFixture`.

		To unskip this and run with `WebServerFixture`:

			NoSkip=true dotnet test Letterbook.Web.Tests/Letterbook.Web.Tests.csproj --filter DisplayName~LandingPageValidationTest

	 */
	[SkipUnlessPortEquals(Settings.DefaultPort)]
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