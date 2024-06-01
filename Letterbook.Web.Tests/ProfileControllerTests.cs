using System.Net;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Web.Tests;

public class ProfileControllerTests : WithMockContext
{
	private Profile _page;
	private readonly Models.Profile _profile;

	public ProfileControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_profile = new FakeProfile("letterbook.example").Generate();
		_page = new Profile(ProfileServiceMock.Object, CoreOptionsMock)
		{
			PageContext = PageContext,
			Handle = null!,
			DisplayName = null!,
			Description = null!,
			CustomFields = []
		};
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_page);
	}

	[Fact(DisplayName = "Should load the template data for a profile")]
	public async Task CanGetPage()
	{
		ProfileServiceAuthMock.Setup(m => m.FindProfiles(It.IsAny<string>())).ReturnsAsync([_profile]);

		var result = await _page.OnGet(_profile.Handle);

		Assert.IsType<PageResult>(result);
		Assert.Equal(_profile.Description, _page.Description.ToString());
		// Might not actually be correct? What about remote profiles?
		Assert.Equal($"@{_profile.Handle}@letterbook.example", _page.Handle);
		Assert.Equal(_profile.DisplayName, _page.DisplayName);
		Assert.Equal(_profile.CustomFields, _page.CustomFields);
	}
}