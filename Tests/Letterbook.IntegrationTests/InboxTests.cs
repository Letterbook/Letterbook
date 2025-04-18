using System.Net;
using System.Net.Http.Headers;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Conversion;
using Bogus;
using Letterbook.Core.Tests;
using Letterbook.IntegrationTests.Fixtures;
using Medo;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.IntegrationTests;

public class InboxTests : IClassFixture<HostFixture<InboxTests>>, ITestSeed, IDisposable
{
	private const string? SkipAuthorization = "Authorization";

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
	private readonly FollowActivity _followActivity;

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

		// activities

		_followActivity = new FollowActivity()
		{
			Actor = _actor.FediId,
			Object = _profiles[7].FediId,
			Id = $"https://{_actor.FediId.Host}/0"
		};
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

	[Fact(DisplayName = "Should reject unsigned POST request")]
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

	[Fact(DisplayName = "Should reject POST from wrong signer")]
	public async Task RejectCreate_WrongSigner()
	{
		const string actor = "https://peer.example/actor/unknown";
		var activity = new CreateActivity()
		{
			Actor = _actor.FediId
		};
		activity.Object.Add(new NoteObject()
		{
			Id = actor + "/note/" + Uuid7.NewUuid7(),
			Content = "<p>This is a simple. but well-formed Create(Note) activity</p>",
			AttributedTo = _actor.FediId
		});
		var content = _serializer.Serialize(activity);
		var payload = new StringContent(content, mediaType: _mediaType);
		// var traceId = Traces.TraceRequest(payload.Headers);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor);
		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact(DisplayName = "Should reject create from wrong actor", Skip = SkipAuthorization)]
	public async Task RejectCreate_WrongActor()
	{
		const string actor = "https://peer.example/actor/unknown";
		var activity = new CreateActivity()
		{
			Actor = _actor.FediId
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

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", _actor.FediId.ToString());
		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact(DisplayName = "Should crawl post from unkown actor")]
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

	[Fact(DisplayName = "Should create a post in reply to a known post")]
	public async Task CreatePost_Reply()
	{
		var actor = _actor.FediId.ToString();
		var activity = new CreateActivity()
		{
			Actor = actor
		};
		activity.Object.Add(new NoteObject()
		{
			Id = actor + "/note/" + Uuid7.NewUuid7(),
			Content = "<p>This is a reply</p>",
			To = _profiles[8].FediId,
			InReplyTo = _host.Posts[_profiles[8]][0].FediId,
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

	[Fact(DisplayName = "Should accept a post in reply to an unknown post")]
	public async Task CreatePost_UnkownReply()
	{
		var actor = _actor.FediId.ToString();
		var activity = new CreateActivity()
		{
			Actor = actor
		};
		activity.Object.Add(new NoteObject()
		{
			Id = actor + "/note/" + Uuid7.NewUuid7(),
			Content = "<p>This is a reply</p>",
			To = actor,
			InReplyTo = actor + "/note/" + Uuid7.NewUuid7(),
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

	[Fact(DisplayName = "Should update a post")]
	public async Task UpdatePost()
	{
		var actor = _actor.FediId.ToString();
		var activity = new UpdateActivity()
		{
			Actor = actor
		};
		activity.Object.Add(new NoteObject()
		{
			Id = _host.Posts[_actor][1].FediId.ToString(),
			Content = "<p>This post has been edited</p>",
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

	[Fact(DisplayName = "Should reject an update to a post if not the author", Skip = SkipAuthorization)]
	public async Task UpdatePost_RejectWrongActor()
	{
		await Task.CompletedTask;
		Assert.Fail("not implemented");
	}

	[Fact(DisplayName = "Should delete a post")]
	public async Task DeletePost()
	{
		var actor = _actor.FediId.ToString();
		var activity = new DeleteActivity()
		{
			Actor = actor
		};
		activity.Object.Add(_host.Posts[_actor][0].FediId);
		var content = _serializer.Serialize(activity);
		var payload = new StringContent(content, mediaType: _mediaType);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor);
		var response = await _client.PostAsync($"actor/{_profiles[8].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
	}

	[Fact(DisplayName = "Should reject a delete to a post if not the author", Skip = SkipAuthorization)]
	public async Task DeletePost_RejectWrongActor()
	{
		await Task.CompletedTask;
		Assert.Fail("not implemented");
	}

	[Fact(DisplayName = "Should accept a follow activity")]
	public async Task CanAcceptFollow()
	{
		var content = _serializer.Serialize(_followActivity);
		var payload = new StringContent(content, mediaType: _mediaType);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", _actor.FediId.ToString());
		var response = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
	}

	[Fact(DisplayName = "Should accept a duplicate follow activity")]
	public async Task CanAcceptFollow_Duplicate()
	{
		var content = _serializer.Serialize(_followActivity);
		var payload = new StringContent(content, mediaType: _mediaType);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", _actor.FediId.ToString());
		var initialResponse = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", payload);
		var secondResponse = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", payload);

		Assert.NotNull(initialResponse);
		Assert.NotNull(secondResponse);
		Assert.Equal(HttpStatusCode.Accepted, initialResponse.StatusCode);
		Assert.Equal(HttpStatusCode.Accepted, secondResponse.StatusCode);
	}

	[Fact(DisplayName = "Should accept undo(follow) activity")]
	public async Task CanAcceptUnfollow()
	{
		var actor = _profiles[12];
		_followActivity.Actor = actor.FediId;
		var undo = new UndoActivity
		{
			Actor = actor.FediId,
			Object = _followActivity
		};
		var content = _serializer.Serialize(undo);
		var payload = new StringContent(content, mediaType: _mediaType);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor.FediId.ToString());
		var response = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact(DisplayName = "Should accept refollow activity after unfollow")]
	public async Task CanAcceptUnfollow_Refollow()
	{
		var actor = _profiles[12];
		_followActivity.Actor = actor.FediId;
		var undo = new UndoActivity
		{
			Actor = actor.FediId,
			Object = _followActivity
		};
		var content = _serializer.Serialize(undo);
		var unfollowPayload = new StringContent(content, mediaType: _mediaType);
		var followPayload = new StringContent(_serializer.Serialize(_followActivity), mediaType: _mediaType);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor.FediId.ToString());
		var unfollowResponse = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", unfollowPayload);
		var followResponse = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", followPayload);

		Assert.NotNull(unfollowResponse);
		Assert.NotNull(followResponse);
		Assert.Equal(HttpStatusCode.OK, unfollowResponse.StatusCode);
		Assert.Equal(HttpStatusCode.Accepted, followResponse.StatusCode);
	}

	[Fact(DisplayName = "Should not accept unhandled activities")]
	public async Task ShouldNotAcceptUnknown()
	{
		var actor = _profiles[12];
		var unknown = new TravelActivity()
		{
			Actor = actor.FediId,
		};
		var content = _serializer.Serialize(unknown);
		var payload = new StringContent(content, mediaType: _mediaType);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", actor.FediId.ToString());
		var response = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
	}

	[Fact(DisplayName = "Should accept a report")]
	public async Task ShouldAcceptReports()
	{
		var fake = new Faker();
		var unknown = new FlagActivity()
		{
			Actor = "https://peer.example/system-actor",
			Object = [_profiles[10].FediId, fake.Internet.Url(), fake.Internet.Url()]
		};
		var content = _serializer.Serialize(unknown);
		var payload = new StringContent(content, mediaType: _mediaType);

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Signed", "https://peer.example/system-actor");
		var response = await _client.PostAsync($"actor/{_profiles[7].Id}/inbox", payload);

		Assert.NotNull(response);
		if (!response.IsSuccessStatusCode) Assert.Fail(await response.Content.ReadAsStringAsync());
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}