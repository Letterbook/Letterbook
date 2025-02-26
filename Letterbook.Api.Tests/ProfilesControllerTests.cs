using Bogus;
using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class ProfilesControllerTests : WithMockContext
{
	private readonly ITestOutputHelper _output;
	private readonly ProfilesController _controller;
	private readonly FakeProfile _fakeProfile;
	private readonly Models.Profile _profile;
	private readonly Guid _accountId;

	public ProfilesControllerTests(ITestOutputHelper output)
	{
		_output = output;
		_output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_accountId = new Faker().Random.Guid();
		Auth(_accountId);
		_controller = new ProfilesController(Mock.Of<ILogger<ProfilesController>>(), CoreOptionsMock, ProfileServiceMock.Object,
			new MappingConfigProvider(CoreOptionsMock), AuthorizationServiceMock.Object)
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};

		_fakeProfile = new FakeProfile(CoreOptionsMock.Value.BaseUri().Authority);
		_profile = _fakeProfile.Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact(DisplayName = "Should get a profile by ID")]
	public async Task CanGetProfile()
	{
		ProfileServiceAuthMock.Setup(m => m.LookupProfile(_profile.Id, (Models.ProfileId?)It.IsAny<Models.ProfileId?>())).ReturnsAsync(_profile);

		var result = await _controller.Get(_profile.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
		Assert.Equal(_profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should create a profile owned by an actor")]
	public async Task CanCreateProfile()
	{
		var account = new FakeAccount().Generate();
		var profile = new FakeProfile(new Uri("https://letterbook.example/actor/new"), account).Generate();
		profile.Handle = "test_handle";
		ProfileServiceAuthMock.Setup(m => m.CreateProfile(account.Id, profile.Handle)).ReturnsAsync(profile);

		var result = await _controller.Create(account.Id, profile.Handle);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
		Assert.Equal(profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should add a custom field to a profile")]
	public async Task CanAddField()
	{
		var expected = new Models.CustomField()
		{
			Label = "test label",
			Value = "test value"
		};
		ProfileServiceAuthMock.Setup(m => m.InsertCustomField(_profile.GetId(), 0, expected.Label, expected.Value))
			.ReturnsAsync(new Models.UpdateResponse<Models.Profile>()
			{
				Original = _profile,
				Updated = _profile
			});

		var result = await _controller.AddField(_profile.GetId(), 0, expected);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
		Assert.Equal(_profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should remove a custom field from a profile")]
	public async Task CanRemoveField()
	{
		ProfileServiceAuthMock.Setup(m => m.RemoveCustomField(_profile.GetId(), 0))
			.ReturnsAsync(new Models.UpdateResponse<Models.Profile>()
			{
				Original = _profile,
				Updated = _profile
			});

		var result = await _controller.RemoveField(_profile.GetId(), 0);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
		Assert.Equal(_profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should update a custom field on a profile")]
	public async Task CanUpdateField()
	{
		var expected = new Models.CustomField()
		{
			Label = "new label",
			Value = "new value"
		};
		ProfileServiceAuthMock.Setup(m => m.UpdateCustomField(_profile.GetId(), 0, expected.Label, expected.Value))
			.ReturnsAsync(new Models.UpdateResponse<Models.Profile>()
			{
				Original = _profile,
				Updated = _profile
			});

		var result = await _controller.UpdateField(_profile.GetId(), 0, expected);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullProfileDto>(response.Value);
		Assert.Equal(_profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should query following")]
	public async Task CanQueryFollowing()
	{
		foreach (var following in _fakeProfile.Generate(5))
		{
			_profile.Follow(following, FollowState.Accepted);
		}
		var queryable = _profile.FollowingCollection.Select(r => r.Follows).BuildMock();
		ProfileServiceAuthMock.Setup(m => m.LookupFollowing(_profile.GetId(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
			.ReturnsAsync(queryable);

		var result = await _controller.GetFollowing(_profile.GetId(), null);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<IAsyncEnumerable<MiniProfileDto>>(response.Value);
		Assert.InRange(await actual.CountAsync(), 5, 100);
	}

	[Fact(DisplayName = "Should query followers")]
	public async Task CanQueryFollowers()
	{
		foreach (var follower in _fakeProfile.Generate(5))
		{
			_profile.AddFollower(follower, FollowState.Accepted);
		}
		var queryable = _profile.FollowersCollection.Select(r => r.Follower).BuildMock();
		ProfileServiceAuthMock.Setup(m => m.LookupFollowers(_profile.GetId(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
			.ReturnsAsync(queryable);

		var result = await _controller.GetFollowers(_profile.GetId(), null);

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<IAsyncEnumerable<MiniProfileDto>>(response.Value);
		Assert.InRange(await actual.CountAsync(), 5, 100);
	}

	[Fact(DisplayName = "Should follow a target profile")]
	public async Task CanFollow()
	{

		var target = _fakeProfile.Generate();
		ProfileServiceAuthMock.Setup(m => m.Follow(_profile.GetId(), target.GetId()))
			.ReturnsAsync(new Models.FollowerRelation(_profile, target, FollowState.Accepted));

		var result = await _controller.Follow(_profile.GetId(), target.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(response.Value);
		Assert.Equal(FollowState.Accepted, actual.State);
	}

	[Fact(DisplayName = "Should accept a follow request")]
	public async Task CanAcceptFollower()
	{
		var target = _fakeProfile.Generate();
		ProfileServiceAuthMock.Setup(m => m.AcceptFollower(_profile.GetId(), target.GetId()))
			.ReturnsAsync(new Models.FollowerRelation(_profile, target, FollowState.Accepted));

		var result = await _controller.AcceptFollower(_profile.GetId(), target.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(response.Value);
		Assert.Equal(FollowState.Accepted, actual.State);
	}

	[Fact(DisplayName = "Should remove a follower")]
	public async Task CanRemoveFollower()
	{
		var target = _fakeProfile.Generate();
		ProfileServiceAuthMock.Setup(m => m.RemoveFollower(_profile.GetId(), target.GetId()))
			.ReturnsAsync(new Models.FollowerRelation(_profile, target, FollowState.None));

		var result = await _controller.RemoveFollower(_profile.GetId(), target.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(response.Value);
		Assert.Equal(FollowState.None, actual.State);
	}

	[Fact(DisplayName = "Should unfollow a target profile")]
	public async Task CanUnfollow()
	{

		var target = _fakeProfile.Generate();
		_profile.Follow(target, FollowState.Accepted);
		ProfileServiceAuthMock.Setup(m => m.Unfollow(_profile.GetId(), target.GetId()))
			.ReturnsAsync(new Models.FollowerRelation(_profile, target, FollowState.None));

		var result = await _controller.Unfollow(_profile.GetId(), target.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(response.Value);
		Assert.Equal(FollowState.None, actual.State);
	}

	[Fact(DisplayName = "Should block a target profile")]
	public async Task CanBlock()
	{

		var target = _fakeProfile.Generate();
		_profile.Follow(target, FollowState.None);
		ProfileServiceAuthMock.Setup(m => m.Block(_profile.GetId(), target.GetId()))
			.ReturnsAsync(new Models.FollowerRelation(_profile, target, FollowState.Blocked));

		var result = await _controller.Block(_profile.GetId(), target.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(response.Value);
		Assert.Equal(FollowState.Blocked, actual.State);
	}

	[Fact(DisplayName = "Should unblock a target profile")]
	public async Task CanUnblock()
	{

		var target = _fakeProfile.Generate();
		_profile.Follow(target, FollowState.Blocked);
		ProfileServiceAuthMock.Setup(m => m.Unblock(_profile.GetId(), target.GetId()))
			.ReturnsAsync(new Models.FollowerRelation(_profile, target, FollowState.None));

		var result = await _controller.Unblock(_profile.GetId(), target.GetId());

		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(response.Value);
		Assert.Equal(FollowState.None, actual.State);
	}
}