using System.Text.RegularExpressions;
using Letterbook.Web.Tests.E2E.Support;

namespace Letterbook.Web.Tests.E2E;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LandingPageValidationTest : PageTest
{
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