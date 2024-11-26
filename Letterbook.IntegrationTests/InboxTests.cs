using System.Net;
using System.Net.Http.Headers;
using ActivityPub.Types.Conversion;
using Letterbook.Core.Tests;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.IntegrationTests;

public class InboxTests : IClassFixture<HostFixture<InboxTests>>, ITestSeed, IDisposable
{
	public void Dispose()
	{
		_host.Dispose();
		_client.Dispose();
		_scope.Dispose();
	}

	private readonly HostFixture<InboxTests> _host;
	private readonly List<Models.Profile> _profiles;
	private readonly HttpClient _client;
	private readonly IJsonLdSerializer _serializer;
	private readonly IServiceScope _scope;
	private Models.Profile _actor;
	private readonly MediaTypeHeaderValue _mediaType = new("application/ld+json");

	public InboxTests(HostFixture<InboxTests> host)
	{
		_host = host;
		_profiles = _host.Profiles;
		_actor = _profiles[13];

		_scope = _host.CreateScope();
		_serializer = _scope.ServiceProvider.GetRequiredService<IJsonLdSerializer>();

		_client = _host.CreateClient(_host.DefaultOptions);
		_client.DefaultRequestHeaders.Accept.Clear();
		_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", _actor.FediId.ToString());
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
	}

	[Fact(DisplayName = "Should create simple post")]
	public async Task CreateSimpleNote()
	{
		var json = TestData.ReadAllText("mastodon_post.json");
		var activity = json.Replace(@"""https://mastodon.castle/users/user""", @$"""{_actor.FediId}""")
			.Replace(@"""https://mastodon.castle/users/user/followers""", @$"""{_actor.Followers}""")
			.Replace("mastodon.castle", _actor.FediId.Host);
		var payload = new StringContent(activity, mediaType: _mediaType);
		// var traceId = Traces.TraceRequest(payload.Headers);

		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}