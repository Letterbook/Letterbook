using System.Security.Claims;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Areas.Administration.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Web.Tests;

public class PeerControllerTests : WithMockContext
{
	private Peer _page;
	private Models.Peer _peer;
	private readonly ClaimsPrincipal _principal;
	private readonly Models.Account _account;

	public PeerControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_account = new FakeAccount().Generate();
		_principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(Jwt.Sub, _account.Id.ToString())], "Test"));
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);
		MockAuthorizeAllowAll();

		_page = new Peer(Mock.Of<ILogger<Peer>>(), ModerationServiceMock.Object, AuthorizationServiceMock.Object)
		{
			PageContext = PageContext
		};
		_peer = new FakePeer().Generate();
		AuthzModerationServiceMock.Setup(m => m.GetOrInitPeer(It.IsAny<Uri>())).ReturnsAsync(_peer);
		AuthzModerationServiceMock.Setup(m => m.UpdatePeer(_peer)).ReturnsAsync(_peer);
		AuthzModerationServiceMock.Setup(m => m.LookupPeer(It.IsAny<Uri>())).ReturnsAsync(_peer);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_page);
	}

	[Fact(DisplayName = "GET should not show expired restrictions")]
	public async Task GetShouldNotShowExpired()
	{
		_peer.Restrictions[Models.Restrictions.Warn] = DateTimeOffset.UtcNow - TimeSpan.FromDays(1);
		_page.Domain = _peer.Hostname;

		var result = await _page.OnGet();

		Assert.IsType<PageResult>(result, exactMatch: false);

		var actual = _page.Form.Restrictions.First(r => r.Id == Models.Restrictions.Warn);
		Assert.False(actual.Enabled);
		Assert.Equal(DateTimeOffset.MaxValue, actual.Expires);
	}

	[Fact(DisplayName = "POST should not show expired restrictions")]
	public async Task PostShouldNotShowExpired()
	{
		_peer.Restrictions[Models.Restrictions.Warn] = DateTimeOffset.UtcNow - TimeSpan.FromDays(1);
		_page.Domain = _peer.Hostname;
		var form = Peer.FormModel.FromPeer(_peer);
		var restriction = form.Restrictions.First(model => model.Id == Models.Restrictions.Warn);
		restriction.Enabled = true;
		restriction.Expires = DateTimeOffset.UtcNow - TimeSpan.FromDays(1);

		_page.Form = form;

		var response = await _page.OnPost();

		Assert.IsType<PageResult>(response, exactMatch: false);

		var actual = _page.Form.Restrictions.First(r => r.Id == Models.Restrictions.Warn);
		Assert.False(actual.Enabled);
		Assert.Equal(DateTimeOffset.MaxValue, actual.Expires);
	}
}