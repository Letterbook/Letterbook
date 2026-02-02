using System.Net;
using System.Net.Http.Json;
using Letterbook.Api.Dto;
using Letterbook.Core;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

public class UnhandledExceptionsTests : IClassFixture<ApiFixture>
{
	private readonly ApiFixture _fixture;
	private readonly Mock<IAccountService> _mockAccountService;

	public UnhandledExceptionsTests(ApiFixture fixture, ITestOutputHelper log)
	{
		_fixture = fixture;
		_mockAccountService = fixture.MockAccountService;
	}

	/*

		Intended to show that unhandled exceptions DO NOT fail with error like:

			 Serialization and deserialization of 'System.Reflection.MethodBase' instances is not supported.

		The above message shows there is a problem somewhere in middleware.

	*/
	[Fact(DisplayName = "Should print stack trace of unhandled exceptions")]
	public async Task UnhandledExceptionsReturnCorrectStackTrace()
	{
		_mockAccountService.Setup(it => it.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(new Models.Account());

		_mockAccountService
			.Setup(it => it.RegisterAccount(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

		var exception = new Exception("An error on purpose");

		_mockAccountService
			.Setup(it => it.AuthenticatePassword(It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(exception);

		using var _client = _fixture.CreateClient();

		var response = await _client.PostAsync("/lb/v1/user_account/register", JsonContent.Create(new RegistrationRequest
		{
			Handle = "example-handle",
			InviteCode = "abc",
			Password = "$Password1",
			ConfirmPassword = "$Password1",
			Email = "anything@domain.com",
		}));

		var body = await response.Content.ReadAsStringAsync();

		Assert.DoesNotContain("Serialization and deserialization of 'System.Reflection.MethodBase' instances is not supported", body);
		Assert.Contains(exception.Message, body);

		// @todo: should really be a problem type returned as json instead of plain text.
	}

	[Fact(Skip= "WIP", DisplayName = "Should return status 500 for unhandled exceptions")]
	public async Task UnhandledExceptionsReturnStatusCode500()
	{
		_mockAccountService.Setup(it => it.LookupAccount(It.IsAny<Guid>())).ReturnsAsync(new Models.Account());

		_mockAccountService
			.Setup(it => it.RegisterAccount(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

		_mockAccountService
			.Setup(it => it.AuthenticatePassword(It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(new Exception());

		using var _client = _fixture.CreateClient();

		var response = await _client.PostAsync("/lb/v1/user_account/register", JsonContent.Create(new RegistrationRequest
		{
			Handle = "example-handle",
			InviteCode = "abc",
			Password = "$Password1",
			ConfirmPassword = "$Password1",
			Email = "anything@domain.com",
		}));

		Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
	}
}