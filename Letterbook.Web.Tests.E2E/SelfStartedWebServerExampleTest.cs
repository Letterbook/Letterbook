using System.Text.RegularExpressions;
using Letterbook.Web.Tests.E2E.Support;

namespace Letterbook.Web.Tests.E2E;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SelfStartedWebServerExampleTest : PageTest
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

	[Test]
	public async Task HomepageHasAFavicon()
	{
		await Page.GotoAsync($"{_webServerFixture.BaseUrl}favicon");
	}

	[Test]
	public async Task HomepageHasCorrectTitleAndLinksToAdminProfile()
	{
		await Page.GotoAsync(_webServerFixture.BaseUrl.ToString());

		await Expect(Page).ToHaveTitleAsync(new Regex("Letterbook.Web"));

		// Get link to admin profile
		var adminProfileLink = Page.GetByRole(AriaRole.Link, new() { Name = "Admin profile" });

		// Expect an attribute "to be strictly equal" to the value.
		await Expect(adminProfileLink).ToHaveAttributeAsync("href", "/@admin");
	}
}