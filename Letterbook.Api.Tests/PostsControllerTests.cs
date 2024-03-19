using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Core.Authorization;
using Letterbook.Core.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Letterbook.Api.Tests;

public class PostsControllerTests : WithMockContext
{
	private readonly PostsController _controller;
	private readonly PostDto _dto;
	private readonly FakeProfile _profileFakes;
	private readonly Models.Profile _profile;


	public PostsControllerTests()
	{
		_controller = new PostsController(Mock.Of<ILogger<PostsController>>(), CoreOptionsMock, PostServiceMock.Object,
			ProfileServiceMock.Object, AuthorizationServiceMock.Object, new MappingConfigProvider(new BaseMappings(CoreOptionsMock)))
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};
		MockAuthorizeAllowAll();
		_dto = new PostDto();
		_profileFakes = new FakeProfile("letterbook.example");
		_profile = _profileFakes.Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact(DisplayName = "Should accept a draft post for a note")]
	public async Task CanDraftNote()
	{
		var result = await _controller.Draft(_profile.GetId25(), _dto);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);
	}
}