using System.Text.RegularExpressions;
using Letterbook.Web.Tests.E2E.Support;

namespace Letterbook.Web.Tests.E2E;

// dotnet test Letterbook.Web.Tests.E2E/Letterbook.Web.Tests.E2E.csproj --filter FullyQualifiedName~Letterbook.Web.Tests.E2E
// dotnet test Letterbook.Web.Tests.E2E/Letterbook.Web.Tests.E2E.csproj --filter Name~LandingPageValidationTest
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LandingPageValidationTest : PageTest
{
	private WebServerFixture _webServerFixture;

	[OneTimeSetUp]
	public async Task BeforeAll()
	{
		_webServerFixture = new WebServerFixture();
		await _webServerFixture.InitializeAsync();
	}

	[OneTimeTearDown]
	public async Task AfterAll()
	{
		await _webServerFixture.DisposeAsync();
	}

	/*

		Useful at the moment because when running against `WebServerFixture` this works but razor pages do not.

		So it is a sanity check to show that the server is running and serving static files.

	 */
	[Test]
	public async Task HomepageHasAFavicon()
	{
		await Page.GotoAsync($"{Settings.BaseUrl}favicon.ico");
	}

	/*

		Help wanted.

		This test is skipped because I can so far *only* get it to pass when running against the default development server:

			BaseUrl=http://localhost:5127 dotnet test Letterbook.Web.Tests.E2E/Letterbook.Web.Tests.E2E.csproj --filter Name~LandingPageValidationTest

		There seems to be a problem with razor pages when running with `WebServerFixture`.

		To unskip this and run with `WebServerFixture`:

			NoSkip=true dotnet test Letterbook.Web.Tests.E2E/Letterbook.Web.Tests.E2E.csproj --filter Name~LandingPageValidationTest

	 */
	[SkipUnlessPortEquals(Settings.DefaultPort)]
	[Test]
	public async Task HomepageHasCorrectTitleAndLinksToAdminProfile()
	{
		await Page.GotoAsync(Settings.BaseUrl.ToString());

		await Expect(Page).ToHaveTitleAsync(new Regex("Letterbook.Web"));

		// Get link to admin profile
		var adminProfileLink = Page.GetByRole(AriaRole.Link, new() { Name = "Admin profile" });

		// Expect an attribute "to be strictly equal" to the value.
		await Expect(adminProfileLink).ToHaveAttributeAsync("href", "/@admin");
	}
}