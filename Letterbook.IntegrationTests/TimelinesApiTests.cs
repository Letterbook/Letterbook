using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.Core.Values;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.IntegrationTests;

// Future Infra: MessageQueue, ObjectStore, Email, BulkEmail, Cache, Backplane
[Trait("Infra", "Postgres")]
[Trait("Infra", "Timescale")]
[Trait("Driver", "Api")]
public class TimelinesApiTests : IClassFixture<HostFixture<TimelinesApiTests>>, ITestSeed
{
	private readonly HostFixture<TimelinesApiTests> _host;
	private readonly HttpClient _client;

	private readonly JsonSerializerOptions _json;

	static int? ITestSeed.Seed() => null;
	static int ITestSeed.TimelineCount() => 500;

	public TimelinesApiTests(HostFixture<TimelinesApiTests> host)
	{
		_host = host;
		_client = _host.CreateClient();

		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
		Assert.NotNull(_client);
	}

	[Fact(DisplayName = "Should return Posts")]
	public async Task CanQuery()
	{
		var response = await _client.GetAsync($"/lb/v1/timelines/{_host.Profiles[0].GetId25()}");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsAssignableFrom<IEnumerable<PostDto>>(
			await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>(_json));

		Assert.NotEmpty(actual);
	}
}