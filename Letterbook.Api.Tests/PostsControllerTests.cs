using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Core.Authorization;
using Letterbook.Core.Tests.Fakes;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class PostsControllerTests : WithMockContext
{
	private readonly PostsController _controller;
	private readonly PostDto _dto;
	private readonly FakeProfile _profileFakes;
	private readonly Models.Profile _profile;
	private readonly FakePost _postFakes;
	private readonly Models.Post _post;


	public PostsControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_controller = new PostsController(Mock.Of<ILogger<PostsController>>(), CoreOptionsMock, PostServiceMock.Object,
			ProfileServiceMock.Object, AuthorizationServiceMock.Object, new MappingConfigProvider(new InstanceMappings(CoreOptionsMock)))
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
		_postFakes = new FakePost(_profile);
		_post = _postFakes.Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact(DisplayName = "Should accept a draft post for a note")]
	public async Task CanDraftNote()
	{
		_dto.Id = _post.GetId();
		PostServiceAuthMock.Setup(m => m.Draft(It.IsAny<Models.Post>(), It.IsAny<Uuid7?>(), It.IsAny<bool>()))
			.ReturnsAsync(_post);

		var result = await _controller.Post(_profile.GetId(), _dto);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);
		Assert.NotEqual(_dto.Id, actual.Id);
	}
}