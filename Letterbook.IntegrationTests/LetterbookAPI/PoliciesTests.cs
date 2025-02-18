using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Bogus;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests.LetterbookAPI;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Api")]
public class PolicyTests : IClassFixture<HostFixture<PolicyTests>>, ITestSeed, IDisposable
{
	private readonly HostFixture<PolicyTests> _host;
	private readonly IServiceScope _scope;
	private readonly Mapper _mapper;
	private readonly JsonSerializerOptions _json;
	private readonly HttpClient _client;
	private readonly Faker _fake;

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_scope.Dispose();
		_client.Dispose();
	}

	public PolicyTests(HostFixture<PolicyTests> host, ITestOutputHelper output)
	{
		_host = host;
		_scope = host.CreateScope();
		_client = _host.CreateClient(_host.DefaultOptions);
		_client.DefaultRequestHeaders.Authorization = new("Test", $"{_host.Accounts[0].Id}");
		_fake = new Faker();

		var mappings = _scope.ServiceProvider.GetRequiredService<MappingConfigProvider>().ModerationReports;
		_mapper = new Mapper(mappings);
		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
		_json.AddDtoSerializer();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
	}

	[Fact(DisplayName = "Should create a new policy")]
	public async Task CanCreate()
	{
		var given = new ModerationPolicyDto
		{
			Title = _fake.Lorem.Sentence(),
			Summary = _fake.Lorem.Sentence(3),
			Policy = _fake.Lorem.Sentence(9)
		};
		var payload = JsonContent.Create(given, options: _json);

		var response = await _client.PostAsync($"/lb/v1/policies/moderator/policy", payload);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<ModerationPolicyDto>(await response.Content.ReadFromJsonAsync<ModerationPolicyDto>(_json));
		Assert.NotNull(actual);
		Assert.Equivalent(given.Summary, actual.Summary);
		Assert.NotEqual(given.Id, actual.Id);
	}

	[Fact(DisplayName = "Should list moderation policies")]
	public async Task CanList()
	{
		var response = await _client.GetAsync($"/lb/v1/policies/public/policy");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsAssignableFrom<IEnumerable<ModerationPolicyDto>>(await response.Content.ReadFromJsonAsync<IEnumerable<ModerationPolicyDto>>(_json));
		Assert.NotNull(actual);
		Assert.Equal(2, actual.Count());
	}

	[Fact(DisplayName = "Should retire a moderation policy")]
	public async Task CanRetire()
	{
		var given = _host.Policies[2];
		var response = await _client.PutAsync($"/lb/v1/policies/moderator/policy/{given.Id}", null);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<ModerationPolicyDto>(await response.Content.ReadFromJsonAsync<ModerationPolicyDto>(_json));
		Assert.NotNull(actual);
		Assert.True(actual.Retired < DateTimeOffset.MaxValue);
	}
}