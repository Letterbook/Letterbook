using Letterbook.Api.Controllers.ActivityPub;
using Letterbook.Api.Tests.Fakes;
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
    private FakeActivity _fakeActivity;
    private FakeActor _fakeActor;
    private FakeProfile _fakeProfile;
    private Profile _profile;
    private AsAp.Activity _activity;
    
    public ActorControllerTests(ITestOutputHelper output)
    {
        _output = output;
        _controller = new ActorController(CoreOptionsMock, Mock.Of<ILogger<ActorController>>(),
            ActivityServiceMock.Object, ProfileServiceMock.Object);

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeActor = new FakeActor();
        _fakeActivity = new FakeActivity(null, _fakeActor.Generate());
        _fakeProfile = new FakeProfile("http://letterbook.example");
        _profile = _fakeProfile.Generate();
        _activity = _fakeActivity.Generate();
    }

    [Fact]
    public async Task Exists()
    {
        Assert.NotNull(_controller);
    }

    [Fact(DisplayName = "Should accept follow activity")]
    [Trait("Inbox", "Follow")]
    public async Task TestFollowAccept()
    {
        _activity.Type = "Follow";

        ProfileServiceMock.Setup(service =>
                service.ReceiveFollowRequest(_profile.LocalId!.Value, _activity.Actor.First().Id!))
            .ReturnsAsync(FollowState.Accepted);

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), _activity);
        
        var actual = response as OkObjectResult;
        var actualObject = actual?.Value as AsAp.Activity;
        Assert.NotNull(actual);
        Assert.NotNull(actualObject);
        Assert.Equal("Accept", actualObject.Type);
    }
    
    [Fact(DisplayName = "Should tentative accept follow activity")]
    [Trait("Inbox", "Follow")]
    public async Task TestFollowTentativeAccept()
    {
        _activity.Type = "Follow";

        ProfileServiceMock.Setup(service =>
                service.ReceiveFollowRequest(_profile.LocalId!.Value, _activity.Actor.First().Id!))
            .ReturnsAsync(FollowState.Pending);

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), _activity);
        
        var actual = response as OkObjectResult;
        var actualObject = actual?.Value as AsAp.Activity;
        Assert.NotNull(actual);
        Assert.NotNull(actualObject);
        Assert.Equal("TentativeAccept", actualObject.Type);
    }
    
    [Fact(DisplayName = "Should reject follow activity")]
    [Trait("Inbox", "Follow")]
    public async Task TestFollowReject()
    {
        _activity.Type = "Follow";

        ProfileServiceMock.Setup(service =>
                service.ReceiveFollowRequest(_profile.LocalId!.Value, _activity.Actor.First().Id!))
            .ReturnsAsync(FollowState.Rejected);

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), _activity);
        
        var actual = response as OkObjectResult;
        var actualObject = actual?.Value as AsAp.Activity;
        Assert.NotNull(actual);
        Assert.NotNull(actualObject);
        Assert.Equal("Reject", actualObject.Type);
    }
    
    [Fact(DisplayName = "Should remove a follower")]
    [Trait("Inbox", "Undo:Follow")]
    public async Task TestUndoFollow()
    {
        var follow = _fakeActivity.Generate();
        follow.Type = "Follow";
        follow.Actor = _activity.Actor;
        _activity.Type = "Undo";
        _activity.Object[0] = follow;

        ProfileServiceMock.Setup(service =>
                service.RemoveFollower(_profile.LocalId!.Value, _activity.Actor.First().Id!))
            .Returns(Task.CompletedTask);

        var response = await _controller.PostInbox(_profile.LocalId!.Value.ToShortId(), _activity);
        
        var actual = response as OkResult;
        Assert.NotNull(actual);
    }
}