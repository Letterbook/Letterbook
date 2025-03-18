using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Letterbook.Core.Tests.Mocks;
using Letterbook.Core.Values;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

public class ClientTests : WithMocks, IClassFixture<JsonLdSerializerFixture>
{
	private Client _client;
	private ITestOutputHelper _output;
	private FakeProfile _fakeProfile;
	private Models.Profile _profile;
	private Models.Profile _targetProfile;

	private readonly MediaTypeWithQualityHeaderValue AcceptHeader = MediaTypeWithQualityHeaderValue
		.Parse("""
               application/ld+json; profile="https://www.w3.org/ns/activitystreams"
               """);


	public ClientTests(ITestOutputHelper output, JsonLdSerializerFixture fixture)
	{
		var httpClient = new HttpClient(HttpMessageHandlerMock.Object);
		_client = new Client(Mock.Of<ILogger<Client>>(), httpClient, fixture.JsonLdSerializer, new Document(fixture.JsonLdSerializer));
		_output = output;

		_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
		_fakeProfile = new FakeProfile("letterbook.example");
		_profile = _fakeProfile.Generate();
		_targetProfile = new FakeProfile().Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_client);
	}

	[Fact(DisplayName = "Should fetch a profile and successfully deserialize it")]
	public async Task FetchProfile()
	{
		await using var fs = TestData.Read("Actor.json");
		var sc = new StreamContent(fs)
		{
			Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
		};
		HttpMessageHandlerMock
			.SetupResponse(r =>
			{
				r.StatusCode = HttpStatusCode.OK;
				r.Content = sc;
			});

		var profile = await _client.As(_profile).Fetch<Models.Profile>(new Uri("http://mastodon.example/users/user"));

		Assert.NotNull(profile);
		Assert.Equal("user", profile.Handle);
	}
}