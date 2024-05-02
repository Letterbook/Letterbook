using System.Xml;
using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Api.Tests.Fakes;
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
	private readonly FakePostDto _fakePostDto;
	private readonly PostDto _dto;
	private readonly FakeProfile _profileFakes;
	private readonly Models.Profile _profile;
	private readonly FakePost _postFakes;
	private readonly Models.Post _post;


	public PostsControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_controller = new PostsController(Mock.Of<ILogger<PostsController>>(), CoreOptionsMock, PostServiceMock.Object,
			ProfileServiceMock.Object, AuthorizationServiceMock.Object, new MappingConfigProvider(CoreOptionsMock))
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};
		MockAuthorizeAllowAll();
		_profileFakes = new FakeProfile("letterbook.example");
		_profile = _profileFakes.Generate();
		_postFakes = new FakePost(_profile);
		_post = _postFakes.Generate();
		_fakePostDto = new FakePostDto(_profile);
		_dto = _fakePostDto.Generate();
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

	[Fact(DisplayName = "Should publish a draft")]
	public async Task CanPublishDraft()
	{
		_dto.Id = _post.GetId();
		PostServiceAuthMock.Setup(m => m.Publish(_post.GetId(), It.IsAny<bool>()))
			.ReturnsAsync(_post);

		var result = await _controller.Publish(_profile.GetId(), _post.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);
		Assert.Equal(_dto.Id, actual.Id);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should update an existing post")]
	public async Task CanUpdatePosts()
	{
		_dto.Id = _post.GetId();
		PostServiceAuthMock.Setup(m => m.Update(_post.GetId(), _post))
			.ReturnsAsync(_post);

		var result = await _controller.Update(_profile.GetId(), _post.GetId(), _dto);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);
		Assert.Equal(_dto.Id, actual.Id);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should attach content to an existing post")]
	public async Task CanAttachContent()
	{
		var dto = new FakeContentDto().Generate();
		PostServiceAuthMock.Setup(m => m.AddContent(_post.GetId(), It.Is<Models.Content>(c => c.GetId() == dto.Id)))
			.ReturnsAsync(_post);

		var result = await _controller.Attach(_profile.GetId(), _post.GetId(), dto);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should edit content in an existing post")]
	public async Task CanEditContent()
	{
		var dto = new FakeContentDto().Generate();
		var id = _post.Contents.First().GetId();
		dto.Id = id;
		PostServiceAuthMock.Setup(m => m.UpdateContent(_post.GetId(), id, It.Is<Models.Content>(c => c.GetId() == dto.Id)))
			.ReturnsAsync(_post);

		var result = await _controller.Edit(_profile.GetId(), _post.GetId(), id, dto);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should remove content in an existing post")]
	public async Task CanRemoveContent()
	{
		PostServiceAuthMock.Setup(m => m.RemoveContent(_post.GetId(), It.IsAny<Uuid7>()))
			.ReturnsAsync(_post);

		var result = await _controller.Remove(_profile.GetId(), _post.GetId(), _post.Contents.First().GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should delete an existing post")]
	public async Task CanDeletePost()
	{
		PostServiceAuthMock.Setup(m => m.Delete(_post.GetId()))
			.Returns(Task.CompletedTask);

		var result = await _controller.Delete(_profile.GetId(), _post.GetId());

		Assert.IsType<OkResult>(result);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}

	[Theory(DisplayName = "Should lookup a post by Id")]
	[InlineData([true])]
	[InlineData([false])]
	public async Task CanGetPost(bool withThread)
	{
		PostServiceAuthMock.Setup(m => m.LookupPost(_post.GetId(), withThread))
			.ReturnsAsync(_post);

		var result = await _controller.Get(_profile.GetId(), _post.GetId(), withThread);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<PostDto>(response.Value);
		Assert.NotNull(actual.FediId);
		Assert.NotNull(actual.Id);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should lookup a thread of posts by Id")]
	public async Task CanGetThread()
	{
		PostServiceAuthMock.Setup(m => m.LookupThread(_post.Thread.GetId()))
			.ReturnsAsync(_post.Thread);

		var result = await _controller.GetThread(_profile.GetId(), _post.Thread.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		Assert.IsAssignableFrom<IEnumerable<PostDto>>(response.Value);

		PostServiceAuthMock.VerifyAll();
		PostServiceAuthMock.VerifyNoOtherCalls();
	}
}