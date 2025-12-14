using System.Security.Claims;
using Bogus;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Controllers;
using Letterbook.Web.Forms;
using Letterbook.Web.Mappers;
using Letterbook.Web.Tests.Fakes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Web.Tests;

public class PostEditorControllerTests : WithMockContext
{
	private readonly PostEditorController _controller;
	private readonly Models.Account _account;
	private readonly ClaimsPrincipal _principal;
	private readonly Models.Profile _selfProfile;

	public Mock<IUrlHelper> UrlMock { get; set; }

	public PostEditorControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_account = new FakeAccount().Generate();
		_selfProfile = new FakeProfile("letterbook.example").Generate();
		_principal = new ClaimsPrincipal(
			new ClaimsIdentity([
					new Claim(Jwt.Sub, _account.Id.ToString()),
					new Claim("activeProfile", _selfProfile.Id.ToString())],
				"Test"));
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);

		UrlMock = MockHelpers.CreateMockUrlHelper(PageContext);
		_controller = new PostEditorController(Mock.Of<ILogger<PostEditorController>>(), PostServiceMock.Object,
			AuthorizationServiceMock.Object, new FormsProfileProvider(CoreOptionsMock))
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			},
			Url = UrlMock.Object
		};
		_controller.Url = UrlMock.Object;
		UrlMock.Setup(m => m.RouteUrl(It.IsAny<UrlRouteContext>()))
			.Returns("fixed-test-url")
			.Verifiable();
		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact]
	public async Task ShouldRedirectToPost()
	{
		var fake = new FakePostEditorFormData();
		var form = fake.Generate();
		var post = new FakePost("letterbook.example").Generate();

		PostServiceAuthMock.Setup(m =>
				m.Draft(It.IsAny<Models.ProfileId>(), It.IsAny<Models.Post>(), It.IsAny<Models.PostId?>(), It.IsAny<bool>()))
			.ReturnsAsync(post);

		var result = await _controller.Submit(new PostEditorForm() { Data = form });

		var actual = Assert.IsType<RedirectToPageResult>(result);
		Assert.False(actual.Permanent);
		// Assert.Equal("/", actual);
	}
}