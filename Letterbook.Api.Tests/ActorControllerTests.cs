using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.Conversion;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.ActivityPub.Mappers;
using Letterbook.Api.Controllers.ActivityPub;
using Letterbook.Api.Tests.Fakes;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;


public class ActorControllerTests : WithMocks
{
    private ITestOutputHelper _output;
    private ActorController _controller;
    // private FakeActivity _fakeActivity;
    private FakeActor _fakeActor;
    private FakeProfile _fakeProfile;
    private FakeProfile _fakeRemoteProfile;
    private Profile _profile;
    private Profile _remoteProfile;
    
    public ActorControllerTests(ITestOutputHelper output)
    {
        _output = output;
        _controller = new ActorController(CoreOptionsMock, Mock.Of<ILogger<ActorController>>(),
            Mock.Of<IJsonLdSerializer>(), ProfileServiceMock.Object, Mock.Of<IActivityMessageService>());

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeActor = new FakeActor();
        // _fakeActivity = new FakeActivity(null, _fakeActor.Generate());
        _fakeProfile = new FakeProfile("http://letterbook.example");
        _fakeRemoteProfile = new FakeProfile();
        _profile = _fakeProfile.Generate();
        _remoteProfile = _fakeRemoteProfile.Generate();
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
        var activity = Activities.BuildActivity(ActivityType.Follow, _remoteProfile);
        activity.Object.Add(_profile.Id);

        ProfileServiceMock.Setup(service =>
                service.ReceiveFollowRequest(_profile.LocalId!.Value, _remoteProfile.Id, It.IsAny<Uri?>()))
            .ReturnsAsync(BuildRelation(FollowState.Accepted));

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), activity);
        
        Assert.IsType<AcceptedResult>(response);
    }
    
    [Fact(DisplayName = "Should tentative accept follow activity")]
    public async Task TestFollowTentativeAccept()
    {
        var activity = Activities.BuildActivity(ActivityType.Follow, _remoteProfile);
        activity.Object.Add(_profile.Id);

        ProfileServiceMock.Setup(service =>
                service.ReceiveFollowRequest(_profile.LocalId!.Value, _remoteProfile.Id, It.IsAny<Uri?>()))
            .ReturnsAsync(BuildRelation(FollowState.Pending));

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), activity);
        
        Assert.IsType<AcceptedResult>(response);
    }
    
    [Fact(DisplayName = "Should reject follow activity")]
    public async Task TestFollowReject()
    {
        var activity = Activities.BuildActivity(ActivityType.Follow, _remoteProfile);
        activity.Object.Add(_profile.Id);

        ProfileServiceMock.Setup(service =>
                service.ReceiveFollowRequest(_profile.LocalId!.Value, _remoteProfile.Id, null))
            .ReturnsAsync(BuildRelation(FollowState.Rejected));

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), activity);
        
        Assert.IsType<AcceptedResult>(response);
    }
    
    [Fact(DisplayName = "Should remove a follower", Skip = "Not implemented")]
    public async Task TestUndoFollow()
    {
        var activity = Activities.BuildActivity(ActivityType.Undo, _remoteProfile, Activities.BuildActivity(ActivityType.Follow, _remoteProfile));

        ProfileServiceMock.Setup(service =>
                service.RemoveFollower(_profile.LocalId!.Value, _remoteProfile.Id))
            .Returns(Task.CompletedTask);

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), activity);
        Assert.IsType<AcceptedResult>(response);
    }
}