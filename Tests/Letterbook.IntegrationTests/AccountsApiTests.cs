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
	private readonly IDataAdapter _dataAdapter;

	public AccountsApiTests(HostFixture<AccountsApiTests> host)
	{
		var clientOptions = new WebApplicationFactoryClientOptions
		{
			BaseAddress = host.Options?.BaseUri() ?? new Uri("localhost:5127"),
			AllowAutoRedirect = false
		};
		_client = host.CreateClient(clientOptions);
		_client.DefaultRequestHeaders.Authorization = new("Test", $"{host.Accounts[0].Id}");

		_dataAdapter = host.DataAdapter;

		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
	}

	[Fact(DisplayName = "Should allow registration")]
	public async Task CanRegister()
	{
		await AddInviteCode("abc");

		var response = await _client.PostAsync("/lb/v1/user_account/register", JsonContent.Create(new RegistrationRequest
		{
			Handle = "example-handle",
			InviteCode = "abc",
			Password = "$Password1",
			ConfirmPassword = "$Password1",
			Email = "anything@domain.com",
		}));

		Assert.True(response.StatusCode == HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
	}

	private async Task AddInviteCode(string value)
	{
		_dataAdapter.Add(new Models.InviteCode(value));

		await _dataAdapter.Commit();
	}
}