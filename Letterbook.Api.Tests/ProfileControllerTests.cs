using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Core.Extensions;
using Letterbook.Core.Tests.Fakes;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class ProfileControllerTests : WithMockContext
{
	private readonly ITestOutputHelper _output;
	private readonly ProfileController _controller;
	private readonly FakeProfile _fakeProfile;
	private readonly Models.Profile _profile;

	public ProfileControllerTests(ITestOutputHelper output)
	{
		_output = output;
		_output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_controller = new ProfileController(Mock.Of<ILogger<ProfileController>>(), CoreOptionsMock, ProfileServiceMock.Object, new MappingConfigProvider(CoreOptionsMock))
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};

		_fakeProfile = new FakeProfile(CoreOptionsMock.Value.BaseUri().Authority);
		_profile = _fakeProfile.Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact(DisplayName = "Should get a profile by ID")]
	public async Task CanGetProfile()
	{
		ProfileServiceAuthMock.Setup(m => m.LookupProfile(_profile.GetId())).ReturnsAsync(_profile);

		var result = await _controller.Get(_profile.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
		Assert.Equal(_profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should get a profile by ID")]
	public async Task CanCreateProfile()
	{
		var account = new FakeAccount().Generate();
		var profile = new FakeProfile(new Uri("https://letterbook.example/actor/new"), account).Generate();
		profile.Handle = "test_handle";
		ProfileServiceAuthMock.Setup(m => m.CreateProfile(account.Id, profile.Handle)).ReturnsAsync(profile);

		var result = await _controller.Create(account.Id, profile.Handle);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
		Assert.Equal(profile.Handle, actual.Handle);
	}
}