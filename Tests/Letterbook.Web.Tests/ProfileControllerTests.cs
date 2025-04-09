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
	private Models.Profile _selfProfile;
	private ClaimsPrincipal _principal;

	public ProfileControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_selfProfile = new FakeProfile("letterbook.example").Generate();
		_principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("activeProfile", _selfProfile.GetId25())], "Test"));

		_profile = new FakeProfile("letterbook.example").Generate();
		_page = new Profile(ProfileServiceMock.Object, Mock.Of<ILogger<Profile>>(), CoreOptionsMock)
		{
			PageContext = PageContext,
		};

		ProfileServiceAuthMock.Setup(m => m.FindProfiles(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<Models.Profile>{_profile}.ToAsyncEnumerable());
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_page);
	}

	[Fact(DisplayName = "Should load the template data for a profile")]
	public async Task CanGetPage()
	{
		var result = await _page.OnGet($"@{_profile.Handle}");

		Assert.IsType<PageResult>(result);
		Assert.Equal(_profile.Description, _page.Description.ToString());
		Assert.Equal($"@{_profile.Handle}@{_profile.Authority}", _page.FullHandle);
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

		await _page.OnGet($"@{_profile.Handle}");

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

		await _page.OnGet($"@{_profile.Handle}");

		Assert.Equal(x, await _page.FollowerCount);
	}

	[Fact(DisplayName = "Should load activeProfile for relationship checks")]
	public async Task CanGetSelf()
	{
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);

		var result = await _page.OnGet($"@{_profile.Handle}");

		Assert.Equal(_selfProfile.GetId25(), _page.SelfId);
	}

	[Fact(DisplayName = "Should perform relationship checks")]
	public async Task CanGetFollowerStatus()
	{
		_selfProfile.Follow(_profile, FollowState.Accepted);
		_selfProfile.AddFollower(_profile, FollowState.Accepted);

		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);
		ProfileServiceAuthMock.Setup(m => m.LookupProfile((Models.ProfileId)_selfProfile.GetId(), (Models.ProfileId?)_profile.GetId()))
			.ReturnsAsync(_selfProfile);

		await _page.OnGet($"@{_profile.Handle}");

		Assert.True(_page.FollowsYou);
		Assert.True(_page.YouFollow);
	}

	[Fact(DisplayName = "Should follow")]
	public async Task CanPostFollowRequest()
	{
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);

		var result = await _page.OnPostFollowRequest(_profile.Handle, _profile.GetId());

		ProfileServiceAuthMock.Verify(m => m.Follow(_selfProfile.GetId(), _profile.GetId()));
	}

	[Fact(DisplayName = "Should unfollow")]
	public async Task CanPostUnfollow()
	{
		_selfProfile.Follow(_profile, FollowState.Accepted);
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);

		var result = await _page.OnPostUnfollow(_profile.Handle, _profile.GetId());

		ProfileServiceAuthMock.Verify(m => m.Unfollow(_selfProfile.GetId(), _profile.GetId()));
	}

	[Fact(DisplayName = "Should remove follower")]
	public async Task CanPostRemoveFollower()
	{
		_profile.Follow(_selfProfile, FollowState.Accepted);
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);

		var result = await _page.OnPostRemoveFollower(_profile.Handle, _profile.GetId());

		ProfileServiceAuthMock.Verify(m => m.RemoveFollower(_selfProfile.GetId(), _profile.GetId()));
	}
}