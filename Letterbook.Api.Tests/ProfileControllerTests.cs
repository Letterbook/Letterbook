using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Letterbook.Api.Tests;

public class ProfileControllerTests : WithMockContext
{
	private readonly ProfileController _controller;

	public ProfileControllerTests()
	{
		_controller = new ProfileController(Mock.Of<ILogger<ProfileController>>(), CoreOptionsMock, ProfileServiceMock.Object);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact(Skip = "Not implemented")]
	public async Task CanGetProfile()
	{
		var result = await _controller.Get(Uuid7.NewUuid7());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
	}
}