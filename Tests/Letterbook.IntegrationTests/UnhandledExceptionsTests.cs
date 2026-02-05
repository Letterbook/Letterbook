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
	private readonly ITestOutputHelper _log;

	public UnhandledExceptionsTests(ApiFixture fixture, ITestOutputHelper log)
	{
		_fixture = fixture;
		_log = log;
	}

	/*

		Intended to show that unhandled exceptions DO NOT fail with error like:

			 Serialization and deserialization of 'System.Reflection.MethodBase' instances is not supported.

		The above message shows there is a problem somewhere in middleware.

		The error was happening because of this line:

			catch (Exception e)
			{
				return BadRequest(e);
			}

		The exception was failing to serialize.

	*/
	[Fact(DisplayName = "Should print stack trace of unhandled exceptions")]
	public async Task UnhandledExceptionsReturnCorrectStackTrace()
	{
		_fixture.MockAccountService
			.Setup(it => it.RegisterAccount(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

		var exception = new Exception("An error on purpose");

		_fixture.MockAccountService
			.Setup(it => it.AuthenticatePassword(It.IsAny<string>(), It.IsAny<string>()))
			.ThrowsAsync(exception);

		_fixture.ReplaceScoped(Mock.Of<IPostService>());

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

		// @todo: should really return a ProblemDetail?.
	}

	[Fact(DisplayName = "Should return status 500 for unhandled exceptions")]
	public async Task UnhandledExceptionsReturnStatusCode500()
	{
		_fixture.MockAccountService
			.Setup(it => it.RegisterAccount(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

		_fixture.MockAccountService
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