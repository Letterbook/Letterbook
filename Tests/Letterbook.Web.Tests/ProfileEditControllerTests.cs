using System.Security.Claims;
using Letterbook.Core.Extensions;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Areas.Profile.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Web.Tests;

public class ProfileEditControllerTests : WithMockContext
{
	private Edit _page;
	private readonly Models.Profile _profile;
	private ClaimsPrincipal _principal;

	public ProfileEditControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_profile = new FakeProfile("letterbook.example").Generate();
		_principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("activeProfile", _profile.Id.ToString())], "Test"));

		_page = new Edit(ProfileServiceMock.Object, CoreOptionsMock)
		{
			PageContext = PageContext,
		};

		ProfileServiceAuthMock.Setup(m => m.LookupProfile(It.IsAny<Models.ProfileId>(), It.IsAny<Models.ProfileId?>()))
			.ReturnsAsync(_profile);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_page);
	}

	[Fact(DisplayName = "Should load the profile edit page")]
	public async Task CanGetPage()
	{
		var result = await _page.OnGet(_profile.Id);

		Assert.IsType<PageResult>(result);
		Assert.Equal(_profile.Description, _page.Description);
		Assert.Equal(_profile.DisplayName, _page.DisplayName);
		var filler = Enumerable.Range(0, CoreOptionsMock.Value.MaxCustomFields - _profile.CustomFields.Length)
			.Select(_ => default(CustomField));
		Assert.Equal(_profile.CustomFields.Select(CustomField.FromModel).ToArray(), _page.CustomFields.WhereNotNull().ToArray());
	}

	[Fact(DisplayName = "Should update the profile")]
	public async Task CanUpdate()
	{
		var expected = new FakeProfile("letterbook.example").Generate();
		expected.Id = _profile.Id;
		_page.CustomFields = expected.CustomFields.Select(CustomField.FromModel).ToArray();
		_page.Description = expected.Description;
		_page.DisplayName = expected.DisplayName;
		var result = await _page.OnPostAsync();

		Assert.IsType<RedirectToPageResult>(result);
	}
}