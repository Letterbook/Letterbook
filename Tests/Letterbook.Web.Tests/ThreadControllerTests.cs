using System.Security.Claims;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;
using Thread = Letterbook.Web.Pages.Thread;

namespace Letterbook.Web.Tests;

public class ThreadControllerTests : WithMockContext
{
	private Thread _page;
	private Models.Profile _selfProfile;
	private Models.Profile _otherProfile;
	private ClaimsPrincipal _principal;
	private FakePost _posts;

	public ThreadControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_selfProfile = new FakeProfile("letterbook.example").Generate();
		_otherProfile = new FakeProfile("letterbook.example").Generate();
		_principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("activeProfile", _selfProfile.Id.ToString())], "Test"));
		_page = new Thread(Mock.Of<ILogger<Thread>>(), ProfileServiceMock.Object, PostServiceMock.Object, new MappingConfigProvider(CoreOptionsMock))
		{
			PageContext = PageContext,
		};

		_posts = new FakePost(_otherProfile);
		ProfileServiceAuthMock.Setup(m => m.LookupProfile(_selfProfile.Id, It.IsAny<Models.ProfileId?>())).ReturnsAsync(_selfProfile);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_page);
	}

	[Fact]
	public async Task CanGetPost()
	{
		var expected = _posts.Generate();
		PostServiceAuthMock.Setup(m => m.LookupPost(expected.Id, true)).ReturnsAsync(expected);

		await _page.OnGet(expected.Id.ToString());

		Assert.Equal(expected, _page.Post);
	}

	[Fact]
	public async Task CanGetThread()
	{
		var expected = _posts.Generate();
		expected.InReplyTo = _posts.Generate();
		expected.RepliesCollection = _posts.Generate(2);
		PostServiceAuthMock.Setup(m => m.LookupPost(expected.Id, true)).ReturnsAsync(expected);

		await _page.OnGet(expected.Id.ToString());

		Assert.Single(_page.Ancestors);
		Assert.Equal(2, _page.Post.RepliesCollection.Count);
	}

	[Fact]
	public async Task CanPostReply()
	{
		var expected = _posts.Generate();
		PostServiceAuthMock.Setup(m => m.LookupPost(expected.Id, true)).ReturnsAsync(expected);
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);

		await _page.OnPost(expected.Id.ToString(), new PostRequestDto
		{
			Contents =
			[
				new ContentDto
				{
					Type = "Note",
					Text = "Test reply",
				}
			],
		});

		PostServiceAuthMock.Verify(m =>
			m.Draft(_selfProfile.Id, It.Is<Models.Post>(p => p.Contents.Any(c => c.Html.Contains("Test reply"))), expected.Id, true));
	}
}