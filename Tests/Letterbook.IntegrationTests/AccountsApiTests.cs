using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Api.Dto;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Postgres")]
[Trait("Infra", "Timescale")]
[Trait("Driver", "Api")]
public class AccountsApiTests : IClassFixture<HostFixture<AccountsApiTests>>, ITestSeed
{
	private readonly HttpClient _client;

	private readonly JsonSerializerOptions _json;
	private readonly Models.InviteCode _inviteCode;

	public AccountsApiTests(HostFixture<AccountsApiTests> host)
	{
		var clientOptions = new WebApplicationFactoryClientOptions
		{
			BaseAddress = host.Options?.BaseUri() ?? new Uri("localhost:5127"),
			AllowAutoRedirect = false
		};
		_client = host.CreateClient(clientOptions);
		_client.DefaultRequestHeaders.Authorization = new("Test", $"{host.Accounts[0].Id}");
		_inviteCode = host.InviteCode;

		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
	}

	/*

		If this fails with:

			Xunit.Sdk.TrueException
            System.ArgumentNullException: Value cannot be null. (Parameter 's')
               at System.ArgumentNullException.Throw(String paramName)
               at System.Text.Encoding.GetBytes(String s)
               at Letterbook.Api.Controllers.UserAccountController.Login(LoginRequest loginRequest) in

		then try this:

			dotnet user-secrets set "HostSecret" "$(openssl rand -base64 32)" --project Source/Letterbook

		See: CONTRIBUTING.md.

		If you'd like to see it fail:

			dotnet user-secrets clear --project Source/Letterbook

	*/
	[Fact(DisplayName = "Should allow registration")]
	public async Task CanRegister()
	{
		var response = await _client.PostAsync("/lb/v1/user_account/register", JsonContent.Create(new RegistrationRequest
		{
			Handle = "example-handle",
			InviteCode = _inviteCode.Code,
			Password = "$Password1",
			ConfirmPassword = "$Password1",
			Email = "anything@domain.com",
		}));

		Assert.True(response.StatusCode == HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
	}
}