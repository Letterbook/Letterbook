using System.Net;
using Letterbook.IntegrationTests.Fixtures;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

public class ProfileLookupTests(ApiFixture fixture, ITestOutputHelper log) : IClassFixture<ApiFixture>
{
	[Fact(DisplayName = "Should invoke search all profile")]
	public async Task InvokeCorrectSearchMethod()
	{
		using var _client = fixture.CreateClient();

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@letterbook.social");

		var body = await response.Content.ReadAsStringAsync();

		log.WriteLine(body);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	// TEST: is authentication required?
}