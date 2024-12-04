using Letterbook.Adapter.ActivityPub;
using Letterbook.Api.Controllers.ActivityPub;
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

	public ActorControllerTests(ITestOutputHelper output)
	{
		_output = output;
		_controller = new ActorController(Mock.Of<ILogger<ActorController>>(), ProfileServiceMock.Object)
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};

		_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
		_fakeProfile = new FakeProfile("http://letterbook.example");
		_fakeRemoteProfile = new FakeProfile();
		_profile = _fakeProfile.Generate();
		_remoteProfile = _fakeRemoteProfile.Generate();
		_document = new Document(JsonLdSerializerMock.Object);
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

		var response = await _controller.PostInbox(_profile.GetId(), activity);

		Assert.IsType<AcceptedResult>(response);
	}

	[Fact(DisplayName = "Should tentative accept follow activity")]
	public async Task TestFollowTentativeAccept()
	{
		var activity = _document.Follow(_remoteProfile, _profile);
		activity.Object.Add(_profile.FediId);

		ProfileServiceAuthMock.Setup(service =>
				service.ReceiveFollowRequest(_profile.GetId(), _remoteProfile.FediId, It.IsAny<Uri?>()))
			.ReturnsAsync(BuildRelation(FollowState.Pending));

		var response = await _controller.PostInbox(_profile.GetId(), activity);

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

		var response = await _controller.PostInbox(_profile.GetId(), activity);

		Assert.IsType<AcceptedResult>(response);
	}
}