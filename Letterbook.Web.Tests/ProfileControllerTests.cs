using System.Security.Claims;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Letterbook.Web.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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
		_page = new Profile(ProfileServiceMock.Object, CoreOptionsMock, Mock.Of<ILogger<Profile>>())
		{
			PageContext = PageContext,
		};

		ProfileServiceAuthMock.Setup(m => m.FindProfiles(It.IsAny<string>())).ReturnsAsync([_profile]);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_page);
	}

	[Fact(DisplayName = "Should load the template data for a profile")]
	public async Task CanGetPage()
	{
		var result = await _page.OnGet(_profile.Handle);

		Assert.IsType<PageResult>(result);
		Assert.Equal(_profile.Description, _page.Description.ToString());
		Assert.Equal($"@{_profile.Handle}@{_profile.Authority}", _page.Handle);
		Assert.Equal(_profile.DisplayName, _page.DisplayName);
		Assert.Equal(_profile.CustomFields, _page.CustomFields);
	}

	[InlineData(0)]
	[InlineData(1)]
	[InlineData(10)]
	[InlineData(1000)]
	[Theory(DisplayName = "Should get the following count")]
	public async Task CanGetFollowing(int x)
	{
		ProfileServiceAuthMock.Setup(m => m.FollowingCount(_profile)).ReturnsAsync(x);

		await _page.OnGet(_profile.Handle);

		Assert.Equal(x, await _page.FollowingCount);
	}

	[InlineData(0)]
	[InlineData(1)]
	[InlineData(10)]
	[InlineData(1000)]
	[Theory(DisplayName = "Should get the follower count")]
	public async Task CanGetFollowers(int x)
	{
		ProfileServiceAuthMock.Setup(m => m.FollowerCount(_profile)).ReturnsAsync(x);

		await _page.OnGet(_profile.Handle);

		Assert.Equal(x, await _page.FollowerCount);
	}

	[Fact(DisplayName = "Should load activeProfile for relationship checks")]
	public async Task CanGetSelf()
	{
		var selfProfile = new FakeProfile("letterbook.example").Generate();
		var identity = new ClaimsIdentity([new Claim("activeProfile", selfProfile.GetId25())], "Test");
		var principal = new ClaimsPrincipal(identity);
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(principal);

		var result = await _page.OnGet(_profile.Handle);

		Assert.Equal(selfProfile.GetId25(), _page.SelfId);
	}

	[Fact(DisplayName = "Should perform relationship checks")]
	public async Task CanGetFollowerStatus()
	{
		var selfProfile = new FakeProfile("letterbook.example").Generate();
		var identity = new ClaimsIdentity([new Claim("activeProfile", selfProfile.GetId25())], "Test");
		var principal = new ClaimsPrincipal(identity);
		selfProfile.Follow(_profile, FollowState.Accepted);
		selfProfile.AddFollower(_profile, FollowState.Accepted);

		MockHttpContext.SetupGet(ctx => ctx.User).Returns(principal);
		ProfileServiceAuthMock.Setup(m => m.LookupProfile(selfProfile.GetId(), _profile.GetId()))
			.ReturnsAsync(selfProfile);

		await _page.OnGet(_profile.Handle);

		Assert.True(_page.FollowsYou);
		Assert.True(_page.YouFollow);
	}
}