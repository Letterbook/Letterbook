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
	private readonly ITestOutputHelper _output;
	private readonly HttpClient _client;
	private readonly List<Models.Profile> _profiles;
	private readonly JsonSerializerOptions _json;

	public WebFingerTests(HostFixture<WebFingerTests> host, ITestOutputHelper output)
	{
		_output = output;

		_client = host.CreateClient();

		_profiles = host.Profiles;

		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
	}

	/*
		@todo: What do we do about the URL prefix '/lb/v1'? Is that hidden at runtime?
		@todo: what does the shape of the reply look like? FullProfileDto or other?
		@todo: what happens if authority does not match the current host?
			Based on manual testing of mastodon.social it returns 404 when authority omitted or does not match the current server host
	*/

	/*

		https://docs.joinmastodon.org/spec/webfinger

		Mastodon uses "acct:" URI scheme (https://datatracker.ietf.org/doc/html/rfc7565) as mentioned
		in https://docs.joinmastodon.org/spec/webfinger/#intro, so doing the same here.

	*/
	[Fact(DisplayName = "Should get a profile by web finger without needing authorization")]
	public async Task CanGetProfileByWebFinger()
	{
		var profile = _profiles[0];

		var response = await _client.SendAsync(
			new HttpRequestMessage(
				HttpMethod.Get, $"/lb/v1/.well-known/webfinger?resource=acct:{profile.Handle}@{profile.Authority}")
			{
				// Authorization is not required
				Headers = { Authorization = AuthenticationHeaderValue.Parse("None") }
			});

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));

		Assert.Equal(profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should return HTTP 404 when the handle does not exist")]
	public async Task WebFingerReturns404NotFoundWhenHandleDoesNotExist()
	{
		var response = await _client.SendAsync(
			new HttpRequestMessage(HttpMethod.Get, "/lb/v1/.well-known/webfinger?resource=acct:xxx-does-not-exist-xxx")
		{
			Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }}
		});

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

		// [!] Don't really expect this as I asked for "application/json"
		Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
	}

	// https://datatracker.ietf.org/doc/html/rfc7033#section-4.2
	[Fact(DisplayName = "Should return HTTP 400 when resource query parameter is invalid")]
	public async Task WebFingerReturns400WhenResourceIsInvalid()
	{
		var profile = _profiles[0];

		var resourceMissingAcctSchemePrefix = $"{profile.Handle}@{profile.Authority}";

		var response = await _client.GetAsync($"/lb/v1/.well-known/webfinger?resource={resourceMissingAcctSchemePrefix}");

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	// https://datatracker.ietf.org/doc/html/rfc7033#section-4.2
	[Fact(DisplayName = "Should return HTTP 400 when resource query parameter is omitted")]
	public async Task WebFingerReturns400WhenResourceIsOmitted()
	{
		var response = await _client.GetAsync("/lb/v1/.well-known/webfinger?resource=");

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

		response = await _client.GetAsync("/lb/v1/.well-known/webfinger");

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	/*
		"...query component MUST include the "resource" parameter exactly once..."

		 -- https://datatracker.ietf.org/doc/html/rfc7033#section-4.2
	*/
	[Fact(DisplayName = "Should return HTTP 400 when resource query parameter is added twice")]
	public async Task WebFingerReturns400WhenResourceIsAddedTwice()
	{
		var response = await _client.GetAsync("/lb/v1/.well-known/webfinger?resource=acct:A&resource=acct:B");

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}