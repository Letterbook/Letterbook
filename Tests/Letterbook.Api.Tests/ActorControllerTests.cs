using System.Security.Claims;
using ActivityPub.Types.AS;
using Bogus;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Api.ActivityPub;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class ActorControllerTests : WithMockContext
{
	private ITestOutputHelper _output;
	private ActorController _controller;
	private FakeProfile _fakeProfile;
	private FakeProfile _fakeRemoteProfile;
	private Profile _profile;
	private Profile _remoteProfile;
	private IActivityPubDocument _document;
	private readonly FakePost _fakePost;
	private readonly Guid _accountId;

	public ActorControllerTests(ITestOutputHelper output)
	{
		_output = output;

		_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
		_fakeProfile = new FakeProfile("letterbook.example");
		_fakeRemoteProfile = new FakeProfile("peer.example");
		_profile = _fakeProfile.Generate();
		_remoteProfile = _fakeRemoteProfile.Generate();
		_document = new Document(JsonLdSerializerMock.Object);
		_fakePost = new FakePost(_profile);

		_accountId = new Faker().Random.Guid();
		var user = Auth(_accountId, new Claim(ApplicationClaims.Actor, _remoteProfile.FediId.ToString()));

		_controller = new ActorController(Mock.Of<ILogger<ActorController>>(), ProfileServiceMock.Object,
			PostServiceMock.Object, ApCrawlerSchedulerMock.Object)
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};
	}

	private FollowerRelation BuildRelation(FollowState state)
	{
		return new FollowerRelation(_remoteProfile, _profile, state);
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact(DisplayName = "Should accept follow activity")]
	public async Task TestFollowAccept()
	{
		var activity = _document.Follow(_remoteProfile, _profile);
		activity.Object.Add(_profile.FediId);

		ProfileServiceAuthMock.Setup(service =>
				service.ReceiveFollowRequest(_profile.Id.Id, _remoteProfile.FediId, It.IsAny<Uri?>()))
			.ReturnsAsync(BuildRelation(FollowState.Accepted));

		var response = await _controller.PostInbox(_profile.Id, activity);

		Assert.IsType<AcceptedResult>(response);
	}

	[Fact(DisplayName = "Should tentative accept follow activity")]
	public async Task TestFollowTentativeAccept()
	{
		var activity = _document.Follow(_remoteProfile, _profile);
		activity.Object.Add(_profile.FediId);

		ProfileServiceAuthMock.Setup(service =>
				service.ReceiveFollowRequest(_profile.Id, _remoteProfile.FediId, It.IsAny<Uri?>()))
			.ReturnsAsync(BuildRelation(FollowState.Pending));

		var response = await _controller.PostInbox(_profile.Id, activity);

		Assert.IsType<AcceptedResult>(response);
	}

	[Fact(DisplayName = "Should reject follow activity")]
	public async Task TestFollowReject()
	{
		var activity = _document.Follow(_remoteProfile, _profile);
		activity.Object.Add(_profile.FediId);

		ProfileServiceAuthMock.Setup(service =>
				service.ReceiveFollowRequest(_profile.Id.Id, _remoteProfile.FediId, null))
			.ReturnsAsync(BuildRelation(FollowState.Rejected));

		var response = await _controller.PostInbox(_profile.Id, activity);

		Assert.IsType<AcceptedResult>(response);
	}

	[Fact(DisplayName = "Should create posts")]
	public async Task TestCreate()
	{
		var post = _fakePost.Generate();
		var activity = _document.Create(_remoteProfile, _document.FromPost(post));

		PostServiceAuthMock.Setup(service => service.ReceiveCreate(It.IsAny<IEnumerable<Post>>()))
			.ReturnsAsync([post]);

		var response = await _controller.PostInbox(_profile.Id, activity);

		Assert.IsType<OkResult>(response);
	}

	[Fact(DisplayName = "Should crawl linked posts")]
	public async Task TestCreate_CrawlLinked()
	{
		var post = _fakePost.Generate();
		var activity = _document.Create(_remoteProfile, _document.FromPost(post));
		activity.Object.Add(new ASLink(){HRef = "https://letterbook.example/post/1"});

		PostServiceAuthMock.Setup(service => service.ReceiveCreate(It.IsAny<IEnumerable<Post>>()))
			.ReturnsAsync([post]);

		var response = await _controller.PostInbox(_profile.Id, activity);

		Assert.IsType<AcceptedResult>(response);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), new Uri("https://letterbook.example/post/1"), It.IsAny<int>()));
	}

	[Fact(DisplayName = "Should accept posts that cannot be created immediately")]
	public async Task TestCreate_AcceptPartial()
	{
		var post = _fakePost.Generate();
		var activity = _document.Create(_remoteProfile, _document.FromPost(post));

		PostServiceAuthMock.Setup(service => service.ReceiveCreate(It.IsAny<IEnumerable<Post>>()))
			.ReturnsAsync([]);

		var response = await _controller.PostInbox(_profile.Id, activity);

		Assert.IsType<AcceptedResult>(response);
	}
}