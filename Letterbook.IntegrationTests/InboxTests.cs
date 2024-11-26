using System.Net;
using System.Net.Http.Headers;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Conversion;
using Letterbook.Core.Tests;
using Letterbook.IntegrationTests.Fixtures;
using Medo;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.IntegrationTests;

public class InboxTests : IClassFixture<HostFixture<InboxTests>>, ITestSeed, IDisposable
{
	public void Dispose()
	{
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

	[Fact(DisplayName = "Should create simple post from Mastodon")]
	public async Task CreatePost_Mastodon()
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

	[Fact(DisplayName = "Should reject unsigned")]
	public async Task RejectPost_Unsigned()
	{
		var activity = new CreateActivity();
		activity.Object.Add(new NoteObject()
		{
			Id = _actor.FediId + "/note/" + Uuid7.NewUuid7(),
			Content = "<p>This is a poorly formed Create(Note) activity!</p><p>It has no actor and is not signed.</p>",
			AttributedTo = _actor.FediId
		});
		var content = _serializer.Serialize(activity);
		var payload = new StringContent(content, mediaType: _mediaType);
		// var traceId = Traces.TraceRequest(payload.Headers);

		_client.DefaultRequestHeaders.Authorization = null;
		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact(DisplayName = "Should accept unkown actor")]
	public async Task AcceptPost_UnknownActor()
	{
		const string actor = "https://peer.example/actor/unknown";
		var activity = new CreateActivity()
		{
			Actor = actor
		};
		activity.Object.Add(new NoteObject()
		{
			Id = actor + "/note/" + Uuid7.NewUuid7(),
			Content = "<p>This is a simple. but well-formed Create(Note) activity</p>",
			AttributedTo = actor
		});
		var content = _serializer.Serialize(activity);
		var payload = new StringContent(content, mediaType: _mediaType);
		// var traceId = Traces.TraceRequest(payload.Headers);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor);
		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
	}

	[Fact(DisplayName = "Should create a post that mentions a known actor")]
	public async Task CreatePost_Mention()
	{
		var actor = _actor.FediId.ToString();
		var activity = new CreateActivity()
		{
			Actor = actor
		};
		activity.Object.Add(new NoteObject()
		{
			Id = actor + "/note/" + Uuid7.NewUuid7(),
			Content = "<p>This is a simple. but well-formed Create(Note) activity</p>",
			To = _profiles[8].FediId,
			AttributedTo = actor
		});
		var content = _serializer.Serialize(activity);
		var payload = new StringContent(content, mediaType: _mediaType);
		// var traceId = Traces.TraceRequest(payload.Headers);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor);
		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact(DisplayName = "Should create a post that mentions an unknown actor")]
	public async Task CreatePost_UnknownMention()
	{
		var actor = _actor.FediId.ToString();
		var activity = new CreateActivity()
		{
			Actor = actor
		};
		activity.Object.Add(new NoteObject()
		{
			Id = actor + "/note/" + Uuid7.NewUuid7(),
			Content = "<p>This is a simple. but well-formed Create(Note) activity</p>",
			To = "https://peer.example/actor/unknown2",
			AttributedTo = actor
		});
		var content = _serializer.Serialize(activity);
		var payload = new StringContent(content, mediaType: _mediaType);
		// var traceId = Traces.TraceRequest(payload.Headers);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor);
		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}