using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.IntegrationTests.Fixtures;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Api")]
public sealed class WebFingerTests : IClassFixture<HostFixture<WebFingerTests>>, ITestSeed
{
	private readonly HostFixture<WebFingerTests> _host;
	private readonly ITestOutputHelper _output;
	private readonly HttpClient _client;
	private readonly List<Models.Profile> _profiles;
	private readonly JsonSerializerOptions _json;
	static int? ITestSeed.Seed() => null;

	public WebFingerTests(HostFixture<WebFingerTests> host, ITestOutputHelper output)
	{
		_host = host;
		_output = output;

		_client = _host.CreateClient();

		_profiles = _host.Profiles;

		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
	}

	/*

		https://docs.joinmastodon.org/spec/webfinger

		@todo: What do we do about the URL prefix '/lb/v1'? Is that hidden at runtime?
		@todo: what does the shape of the reply look like? FullProfileDto or other?
		@todo: there should be no authorization required
		@todo: Is there a specific shape that the resource parameter needs to take?

	*/
	[Fact(DisplayName = "Should get a profile by web finger")]
	public async Task CanGetProfileByWebFinger()
	{
		var profile = _profiles[0];

		var response = await _client.GetAsync($"/lb/v1/.well-known/webfinger?resource={profile.Handle}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var body = await response.Content.ReadAsStringAsync();

		_output.WriteLine(body);

		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));

		Assert.Equal(profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should return 404 when no handle found")]
	public async Task WebFingerReturns404NotFoundWhenHandleDoesNotExist()
	{
		var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/lb/v1/.well-known/webfinger?resource=xxx-does-not-exist-xxx")
		{
			Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }}
		});

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

		// [!] Don't really expect this as I asked for "application/json"
		Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
	}
}