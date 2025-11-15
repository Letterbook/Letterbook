using System.Net;
using System.Net.Http.Json;
using Letterbook.Core.Models.Dto;

namespace Letterbook.IntegrationTests.LetterbookAPI;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Api")]
public partial class TrustAndSafetyTests
{
	[Fact(DisplayName = "Should create a new policy")]
	public async Task CanCreatePolicy()
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