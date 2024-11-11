using Playwright = Letterbook.Web.Tests.E2E.Support.Playwright;

namespace Letterbook.Web.Tests.E2E;

[SetUpFixture]
public class OneTimeSetup
{
	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		Playwright.Install();
	}
}
