using System.Security.Claims;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace Letterbook.Web.Tests;

public class AccountControllerTests : WithMockContext
{
	private Account _page;
	private Models.Account _account;
	private readonly ClaimsPrincipal _principal;
	private Mock<IUrlHelper> UrlMock;

	public AccountControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		UrlMock = MockHelpers.CreateMockUrlHelper(PageContext);
		_account = new FakeAccount().Generate();
		_principal = new ClaimsPrincipal(new ClaimsIdentity([new Claim(Jwt.Sub, _account.Id.ToString())], "Test"));
		_page = new Account(AccountServiceMock.Object, Mock.Of<ILogger<Account>>())
		{
			PageContext = PageContext,
			Url = UrlMock.Object
		};
		_page.Url = UrlMock.Object;
		AccountServiceMock.Setup(m => m.LookupAccount(_account.Id)).ReturnsAsync(_account);
		MockHttpContext.SetupGet(ctx => ctx.User).Returns(_principal);
		UrlMock.Setup(m => m.RouteUrl(It.IsAny<UrlRouteContext>()))
			.Returns("fixed-test-url")
			.Verifiable();
		RouteValues["page"] = "Account";
		RouteData.Values["page"] = "Account";
	}

	[Fact]
	public void ShouldExist()
	{
		Assert.NotNull(_page);
	}

	[Fact]
	public async Task ShouldGetPage()
	{
		var result = await _page.OnGet();

		Assert.IsType<PageResult>(result);
		Assert.Equal(_account.Name, _page.DisplayName);
		Assert.Equal(_account.Email, _page.Email);
		Assert.Null(_page.PageAction);
	}

	[Fact]
	public async Task ShouldGetPage_ConfirmEmail()
	{
		AccountServiceMock.Setup(m => m.ChangeEmailWithToken("old@email.example", "new@email.example", "some-token"))
			.ReturnsAsync(IdentityResult.Success);
		_page.PageAction = "ConfirmEmail";
		var result = await _page.OnGet("some-token", "old@email.example", "new@email.example");

		Assert.IsType<PageResult>(result);
		Assert.Equal(_account.Name, _page.DisplayName);
		Assert.Equal(_account.Email, _page.Email);
		Assert.Equal("Success", _page.ConfirmEmailResult);
	}

	[Fact]
	public async Task ShouldPostEmail()
	{
		AccountServiceMock.Setup(m => m.GenerateChangeEmailToken(_account.Id, "new@email.example")).ReturnsAsync("some-token");

		var result = await _page.OnPostEmail("new@email.example", _account.Email!);

		Assert.IsType<PageResult>(result);
		Assert.Equal(_account.Name, _page.DisplayName);
		Assert.Equal(_account.Email, _page.Email);
		UrlMock.Verify();
	}
}