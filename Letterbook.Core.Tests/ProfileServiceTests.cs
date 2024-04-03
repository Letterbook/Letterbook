using System.Net;
using Bogus;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Medo;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class ProfileServiceTests : WithMocks
{
	private ITestOutputHelper _output;
	private ProfileService _service;
	private FakeAccount _fakeAccount;
	private FakeProfile _fakeProfile;
	private Profile _profile;

	public ProfileServiceTests(ITestOutputHelper output)
	{
		_output = output;
		_output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_fakeAccount = new FakeAccount();
		_fakeProfile = new FakeProfile("letterbook.example");
		CoreOptionsMock.Value.MaxCustomFields = 2;

		_service = new ProfileService(Mock.Of<ILogger<ProfileService>>(), CoreOptionsMock, AccountProfileMock.Object,
			Mock.Of<IProfileEventService>(), ActivityPubClientMock.Object);
		_profile = _fakeProfile.Generate();
	}

	[Fact]
	public void ShouldExist()
	{
		Assert.NotNull(_service);
	}

	[Fact(DisplayName = "Should create a new profile")]
	public async Task CreateNewProfile()
	{
		var accountId = Uuid7.NewUuid7();
		var expected = "testAccount";
		AccountProfileMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(_fakeAccount.Generate());
		AccountProfileMock.Setup(m => m.AnyProfile(expected)).ReturnsAsync(false);

		var actual = await _service.CreateProfile(accountId, expected);

		Assert.NotNull(actual);
		Assert.Equal(expected, actual.Handle);
	}

	[Fact(DisplayName = "Should not create an orphan profile")]
	public async Task NoCreateOrphanProfile()
	{
		var accountId = Uuid7.NewUuid7();
		var expected = "testAccount";
		AccountProfileMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(default(Account));
		AccountProfileMock.Setup(m => m.AnyProfile(expected)).ReturnsAsync(false);

		await Assert.ThrowsAsync<CoreException>(async () => await _service.CreateProfile(accountId, expected));
	}

	[Fact(DisplayName = "Should not create a duplicate profile")]
	public async Task NoCreateDuplicate()
	{
		var accountId = Uuid7.NewUuid7();
		var expected = "testAccount";
		var existing = _fakeProfile.Generate();
		existing.Handle = expected;
		AccountProfileMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(_fakeAccount.Generate());
		AccountProfileMock.Setup(m => m.AnyProfile(It.IsAny<string>())).ReturnsAsync(true);

		await Assert.ThrowsAsync<CoreException>(async () => await _service.CreateProfile(accountId, expected));
	}

	[Fact(DisplayName = "Should update the display name")]
	public async Task UpdateDisplayName()
	{
		var expectedId = Uuid7.NewUuid7();
		_profile.Id = expectedId;
		_profile.DisplayName = new Faker().Internet.UserName();
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(_profile);

		var actual = await _service.UpdateDisplayName(expectedId, "Test Name");

		// Assert.NotEqual(_profile.LocalId, expectedId);
		Assert.NotNull(actual.Updated);
		Assert.Equal("Test Name", actual.Updated.DisplayName);
	}

	[Fact(DisplayName = "Should not update the display name when the profile doesn't exist")]
	public async Task NoUpdateDisplayNameNotExists()
	{
		var expectedId = Uuid7.NewUuid7();
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(default(Profile));

		await Assert.ThrowsAsync<CoreException>(() => _service.UpdateDisplayName(expectedId, "Test Name"));
	}

	[Fact(DisplayName = "Should not update the display name when the name is unchanged")]
	public async Task NoUpdateDisplayNameUnchanged()
	{
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		var actual = await _service.UpdateDisplayName((Uuid7)_profile.Id!, _profile.DisplayName);

		Assert.Null(actual.Updated);
		Assert.Equal(_profile, actual.Original);
		Assert.Equal(_profile.DisplayName, actual.Original.DisplayName);
		Assert.NotNull(actual.Original.DisplayName);
	}

	[Fact(DisplayName = "Should update the bio")]
	public async Task UpdateBio()
	{
		var expectedId = Uuid7.NewUuid7();
		_profile.Id = expectedId;
		_profile.DisplayName = new Faker().Internet.UserName();
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(_profile);

		var actual = await _service.UpdateDescription(expectedId, "This is a test user bio");

		Assert.NotNull(actual.Updated);
		Assert.Equal("This is a test user bio", actual.Updated.Description);
	}

	[Fact(DisplayName = "Should not update the bio when the profile doesn't exist")]
	public async Task NoUpdateBioNotExists()
	{
		var expectedId = Uuid7.NewUuid7();
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(default(Profile));

		await Assert.ThrowsAsync<CoreException>(() =>
			_service.UpdateDescription(expectedId, "This is a test user bio"));
	}

	[Fact(DisplayName = "Should not update the bio when it is unchanged")]
	public async Task NoUpdateBioUnchanged()
	{
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		var actual = await _service.UpdateDescription((Uuid7)_profile.Id!, _profile.Description);

		Assert.Null(actual.Updated);
		Assert.Equal(_profile, actual.Original);
		Assert.Equal(_profile.Description, actual.Original.Description);
		Assert.NotNull(actual.Original.Description);
	}

	[Fact(DisplayName = "Should insert new custom fields")]
	public async Task InsertCustomField()
	{
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		var actual = await _service.InsertCustomField((Uuid7)_profile.Id!, 0, "test item", "test value");
		// var (original, actual) = await _service.InsertCustomField((Uuid7)_profile.LocalId!, 0, "test item", "test value");

		// Assert.NotEqual(_profile.LocalId, expectedId);
		Assert.NotNull(actual.Updated);
		Assert.Equal("test item", actual.Updated.CustomFields[0].Label);
		Assert.Equal("test value", actual.Updated.CustomFields[0].Value);
		Assert.NotEqual("test value", actual.Original.CustomFields[0].Value);
		Assert.NotEqual("test item", actual.Original.CustomFields[0].Label);
		Assert.Equal(2, actual.Updated.CustomFields.Length);
	}

	[Fact(DisplayName = "Should insert new custom fields at given index")]
	public async Task InsertCustomFieldAtIndex()
	{
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		var actual = await _service.InsertCustomField((Uuid7)_profile.Id!, 1, "test item", "test value");
		// var (original, actual) = await _service.InsertCustomField((Uuid7)_profile.LocalId!, 1, "test item", "test value");

		// Assert.NotEqual(_profile.LocalId, expectedId);
		Assert.NotNull(actual.Updated);
		Assert.Equal("test item", actual.Updated.CustomFields[1].Label);
		Assert.Equal("test value", actual.Updated.CustomFields[1].Value);
		Assert.NotEqual(actual.Updated.CustomFields[1].Label, actual.Original.CustomFields[0].Value);
		Assert.NotEqual(actual.Updated.CustomFields[1].Value, actual.Original.CustomFields[0].Label);
		Assert.Equal(2, actual.Updated.CustomFields.Length);
	}

	[Fact(DisplayName = "Should not insert custom fields when the profile doesn't exist")]
	public async Task NoInsertCustomField()
	{
		var expectedId = Uuid7.NewUuid7();
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(default(Profile));

		await Assert.ThrowsAsync<CoreException>(() =>
			_service.InsertCustomField((Uuid7)_profile.Id!, 0, "test item", "test value"));
	}

	[Fact(DisplayName = "Should not insert custom fields when the list is already full")]
	public async Task NoInsertCustomFieldTooMany()
	{
		_profile.CustomFields = _profile.CustomFields.Append(new() { Label = "item2", Value = "value2" }).ToArray();
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		await Assert.ThrowsAsync<CoreException>(() =>
			_service.InsertCustomField((Uuid7)_profile.Id!, 0, "test item", "test value"));
	}

	[Fact(DisplayName = "Should update custom fields")]
	public async Task UpdateCustomField()
	{
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		var actual = await _service.UpdateCustomField((Uuid7)_profile.Id!, 0, "test item", "test value");

		Assert.NotNull(actual.Updated);
		Assert.Equal("test item", actual.Updated.CustomFields[0].Label);
		Assert.Equal("test value", actual.Updated.CustomFields[0].Value);
		Assert.Single(actual.Updated.CustomFields);
	}

	[Fact(DisplayName = "Should delete custom fields")]
	public async Task DeleteCustomField()
	{
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		var actual = await _service.RemoveCustomField((Uuid7)_profile.Id!, 0);

		Assert.NotNull(actual.Updated);
		Assert.Empty(actual.Updated.CustomFields);
	}

	[Fact(DisplayName = "Should add local follows")]
	public async Task FollowLocalProfile()
	{
		var target = _fakeProfile.Generate();
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(
			It.Is<Uuid7>(self => self == (Uuid7)_profile.Id!),
			It.Is<Uri>(uri => uri == target.FediId)
		)).ReturnsAsync(_profile);
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(self => self == (Uuid7)target.Id!)))
			.ReturnsAsync(target);

		var actual = await _service.Follow((Uuid7)_profile.Id!, (Uuid7)target.Id!);

		Assert.Equal(FollowState.Accepted, actual);
		Assert.Contains(target, _profile.FollowingCollection.Select(r => r.Follows));
	}

	[Fact(DisplayName = "Should add local follows by URL")]
	public async Task FollowLocalProfileUrl()
	{
		var target = _fakeProfile.Generate();
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(
			It.Is<Uuid7>(self => self == (Uuid7)_profile.Id!),
			It.Is<Uri>(uri => uri == target.FediId)
		)).ReturnsAsync(_profile);
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uri>(self => self == target.FediId)))
			.ReturnsAsync(target);

		var actual = await _service.Follow((Uuid7)_profile.Id!, target.FediId);

		Assert.Equal(FollowState.Accepted, actual);
		Assert.Contains(target, _profile.FollowingCollection.Select(r => r.Follows));
	}

	[Fact(DisplayName = "Should add remote follows accepted")]
	public async Task FollowRemoteAccept()
	{
		var target = new FakeProfile().Generate();
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(
			It.Is<Uuid7>(self => self == (Uuid7)_profile.Id!),
			It.Is<Uri>(uri => uri == target.FediId)
		)).ReturnsAsync(_profile);
		AccountProfileMock.Setup(m => m.LookupProfile(target.FediId))
			.ReturnsAsync(default(Profile));
		ActivityPubAuthClientMock.Setup(m => m.Fetch<Profile>(target.FediId)).ReturnsAsync(target);
		ActivityPubAuthClientMock.Setup(m => m.SendFollow(target.Inbox, target))
			.ReturnsAsync(new ClientResponse<FollowState>()
			{
				Data = FollowState.Accepted,
				StatusCode = HttpStatusCode.OK,
				DeliveredAddress = target.Inbox
			});

		var actual = await _service.Follow((Uuid7)_profile.Id!, target.FediId);

		Assert.Equal(FollowState.Accepted, actual);
		Assert.Contains(target, _profile.FollowingCollection.Select(r => r.Follows));
	}

	[Fact(DisplayName = "Should add remote follows pending")]
	public async Task FollowRemotePending()
	{
		var target = new FakeProfile().Generate();
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(
			It.Is<Uuid7>(self => self == (Uuid7)_profile.Id!),
			It.Is<Uri>(uri => uri == target.FediId)
		)).ReturnsAsync(_profile);
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uri>(self => self == target.FediId)))
			.ReturnsAsync(default(Profile));
		ActivityPubAuthClientMock.Setup(m => m.Fetch<Profile>(target.FediId)).ReturnsAsync(target);
		ActivityPubAuthClientMock.Setup(m => m.SendFollow(target.Inbox, target))
			.ReturnsAsync(new ClientResponse<FollowState>()
			{
				Data = FollowState.Pending,
				StatusCode = HttpStatusCode.OK,
				DeliveredAddress = target.Inbox
			});

		var actual = await _service.Follow((Uuid7)_profile.Id!, target.FediId);

		Assert.Equal(FollowState.Pending, actual);
		Assert.Contains(target, _profile.FollowingCollection.Select(r => r.Follows));
	}

	[Fact(DisplayName = "Should not add rejected remote follows")]
	public async Task FollowRemoteRejected()
	{
		var target = new FakeProfile().Generate();
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(
			It.Is<Uuid7>(self => self == (Uuid7)_profile.Id!),
			It.Is<Uri>(uri => uri == target.FediId)
		)).ReturnsAsync(_profile);
		AccountProfileMock.Setup(m => m.LookupProfile(It.Is<Uri>(self => self == target.FediId)))
			.ReturnsAsync(default(Profile));
		ActivityPubAuthClientMock.Setup(m => m.Fetch<Profile>(target.FediId)).ReturnsAsync(target);
		ActivityPubAuthClientMock.Setup(m => m.SendFollow(target.Inbox, target))
			.ReturnsAsync(new ClientResponse<FollowState>()
			{
				Data = FollowState.Rejected,
				StatusCode = HttpStatusCode.OK,
				DeliveredAddress = target.Inbox
			});

		var actual = await _service.Follow((Uuid7)_profile.Id!, target.FediId);

		Assert.Equal(FollowState.Rejected, actual);
		Assert.Empty(_profile.FollowingCollection);
		Assert.Empty(target.FollowersCollection);
	}

	[Fact(DisplayName = "Should add a new follower")]
	public async Task ReceiveFollowRequest()
	{
		var follower = new FakeProfile().Generate();
		AccountProfileMock.Setup(m => m.LookupProfile(_profile.FediId)).ReturnsAsync(_profile);
		AccountProfileMock.Setup(m => m.LookupProfile(follower.FediId)).ReturnsAsync(follower);

		var actual = await _service.ReceiveFollowRequest(_profile.FediId, follower.FediId, null);

		Assert.Equal(FollowState.Accepted, actual.State);
		Assert.Contains(follower, _profile.FollowersCollection.Select(r => r.Follower));
	}

	[Fact(DisplayName = "Should update a pending follow")]
	public async Task FollowReply()
	{
		var target = new FakeProfile().Generate();
		_profile.Follow(target, FollowState.Pending);
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(_profile.FediId, target.FediId)).ReturnsAsync(_profile);

		var actual = await _service.ReceiveFollowReply(_profile.FediId, target.FediId, FollowState.Accepted);

		Assert.Equal(FollowState.Accepted, actual);
		Assert.Equal(FollowState.Accepted,
			_profile.FollowingCollection.FirstOrDefault(r => r.Follows.FediId == target.FediId)?.State);
	}

	[Fact(DisplayName = "Should remove a pending follow on reject")]
	public async Task FollowReplyReject()
	{
		var target = new FakeProfile().Generate();
		_profile.Follow(target, FollowState.Pending);
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(_profile.FediId, target.FediId)).ReturnsAsync(_profile);

		var actual = await _service.ReceiveFollowReply(_profile.FediId, target.FediId, FollowState.Rejected);

		Assert.Equal(FollowState.None, actual);
		Assert.DoesNotContain(target, _profile.FollowingCollection.Select(r => r.Follows));

		// Assert.Equal(FollowState.Accepted, _profile.Following.FirstOrDefault(r => r.Follows.Id == target.Id)?.State);
	}

	[Fact(DisplayName = "Should remove a follower")]
	public async Task RemoveFollower()
	{
		var follower = new FakeProfile().Generate();
		_profile.AddFollower(follower, FollowState.Accepted);
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(_profile.GetId(), follower.FediId))
			.ReturnsAsync(_profile);

		await _service.RemoveFollower(_profile.GetId(), follower.FediId);

		Assert.DoesNotContain(follower, _profile.FollowersCollection.Select(r => r.Follower));
	}

	[Fact(DisplayName = "Should unfollow")]
	public async Task Unfollow()
	{
		var follower = new FakeProfile().Generate();
		_profile.Follow(follower, FollowState.Accepted);
		AccountProfileMock.Setup(m => m.LookupProfileWithRelation(_profile.GetId(), follower.FediId))
			.ReturnsAsync(_profile);

		await _service.Unfollow(_profile.GetId(), follower.FediId);

		Assert.DoesNotContain(follower, _profile.FollowingCollection.Select(r => r.Follows));
	}
}