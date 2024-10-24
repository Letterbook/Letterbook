using System.Net;
using System.Security.Cryptography;
using Bogus;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Medo;
using Microsoft.Extensions.Logging;
using MockQueryable;
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

		_service = new ProfileService(Mock.Of<ILogger<ProfileService>>(), CoreOptionsMock, DataAdapterMock.Object,
			Mock.Of<IProfileEventPublisher>(), ActivityPubClientMock.Object, Mock.Of<IHostSigningKeyProvider>(), ActivityPublisherMock.Object);
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
		DataAdapterMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(_fakeAccount.Generate());
		DataAdapterMock.Setup(m => m.AnyProfile(expected)).ReturnsAsync(false);

		var actual = await _service.CreateProfile(accountId, expected);

		Assert.NotNull(actual);
		Assert.Equal(expected, actual.Handle);
	}

	[Fact(DisplayName = "Should not create an orphan profile")]
	public async Task NoCreateOrphanProfile()
	{
		var accountId = Uuid7.NewUuid7();
		var expected = "testAccount";
		DataAdapterMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(default(Account));
		DataAdapterMock.Setup(m => m.AnyProfile(expected)).ReturnsAsync(false);

		await Assert.ThrowsAsync<CoreException>(async () => await _service.CreateProfile(accountId, expected));
	}

	[Fact(DisplayName = "Should not create a duplicate profile")]
	public async Task NoCreateDuplicate()
	{
		var accountId = Uuid7.NewUuid7();
		var expected = "testAccount";
		var existing = _fakeProfile.Generate();
		existing.Handle = expected;
		DataAdapterMock.Setup(m => m.LookupAccount(accountId)).ReturnsAsync(_fakeAccount.Generate());
		DataAdapterMock.Setup(m => m.AnyProfile(It.IsAny<string>())).ReturnsAsync(true);

		await Assert.ThrowsAsync<CoreException>(async () => await _service.CreateProfile(accountId, expected));
	}

	[Fact(DisplayName = "Should update the display name")]
	public async Task UpdateDisplayName()
	{
		var expectedId = Uuid7.NewUuid7();
		_profile.Id = expectedId;
		_profile.DisplayName = new Faker().Internet.UserName();
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
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
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(default(Profile));

		await Assert.ThrowsAsync<CoreException>(() => _service.UpdateDisplayName(expectedId, "Test Name"));
	}

	[Fact(DisplayName = "Should not update the display name when the name is unchanged")]
	public async Task NoUpdateDisplayNameUnchanged()
	{
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
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
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(_profile);

		var actual = await _service.UpdateDescription(expectedId, "This is a test user bio");

		Assert.NotNull(actual.Updated);
		Assert.Equal("This is a test user bio", actual.Updated.Description);
	}

	[Fact(DisplayName = "Should not update the bio when the profile doesn't exist")]
	public async Task NoUpdateBioNotExists()
	{
		var expectedId = Uuid7.NewUuid7();
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(default(Profile));

		await Assert.ThrowsAsync<CoreException>(() =>
			_service.UpdateDescription(expectedId, "This is a test user bio"));
	}

	[Fact(DisplayName = "Should not update the bio when it is unchanged")]
	public async Task NoUpdateBioUnchanged()
	{
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
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
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
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
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
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
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == expectedId)))
			.ReturnsAsync(default(Profile));

		await Assert.ThrowsAsync<CoreException>(() =>
			_service.InsertCustomField((Uuid7)_profile.Id!, 0, "test item", "test value"));
	}

	[Fact(DisplayName = "Should not insert custom fields when the list is already full")]
	public async Task NoInsertCustomFieldTooMany()
	{
		_profile.CustomFields = _profile.CustomFields.Append(new() { Label = "item2", Value = "value2" }).ToArray();
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		await Assert.ThrowsAsync<CoreException>(() =>
			_service.InsertCustomField((Uuid7)_profile.Id!, 0, "test item", "test value"));
	}

	[Fact(DisplayName = "Should update custom fields")]
	public async Task UpdateCustomField()
	{
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
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
		DataAdapterMock.Setup(m => m.LookupProfile(It.Is<Uuid7>(given => given == _profile.Id)))
			.ReturnsAsync(_profile);

		var actual = await _service.RemoveCustomField((Uuid7)_profile.Id!, 0);

		Assert.NotNull(actual.Updated);
		Assert.Empty(actual.Updated.CustomFields);
	}

	[Fact(DisplayName = "Should add local follows")]
	public async Task FollowLocalProfile()
	{
		var target = _fakeProfile.Generate();
		DataAdapterMock.Setup(m => m.SingleProfile(_profile.GetId())).Returns(new List<Profile>{_profile}.BuildMock());
		DataAdapterMock.Setup(m => m.SingleProfile(target.GetId())).Returns(new List<Profile>{target}.BuildMock());

		var actual = await _service.Follow(_profile.GetId(), target.GetId());

		Assert.Equal(FollowState.Accepted, actual.State);
		Assert.Contains(target, _profile.FollowingCollection.Select(r => r.Follows));
	}

	[Fact(DisplayName = "Should add local follows by URL")]
	public async Task FollowLocalProfileUrl()
	{
		var target = _fakeProfile.Generate();
		DataAdapterMock.Setup(m => m.SingleProfile(_profile.GetId())).Returns(new List<Profile>{_profile}.BuildMock());
		DataAdapterMock.Setup(m => m.SingleProfile(target.FediId)).Returns(new List<Profile>{target}.BuildMock());

		var actual = await _service.Follow((Uuid7)_profile.Id!, target.FediId);

		Assert.Equal(FollowState.Accepted, actual.State);
		Assert.Contains(target, _profile.FollowingCollection.Select(r => r.Follows));
	}

	[Fact(DisplayName = "Should add remote follows pending")]
	public async Task FollowRemotePending()
	{
		var target = new FakeProfile().Generate();
		DataAdapterMock.Setup(m => m.SingleProfile(_profile.GetId())).Returns(new List<Profile>{_profile}.BuildMock());
		DataAdapterMock.Setup(m => m.SingleProfile(target.FediId)).Returns(new List<Profile>().BuildMock());
		ActivityPubAuthClientMock.Setup(m => m.Fetch<Profile>(target.FediId)).ReturnsAsync(target);

		var actual = await _service.Follow((Uuid7)_profile.Id!, target.FediId);

		Assert.Equal(FollowState.Pending, actual.State);
		Assert.Contains(target, _profile.FollowingCollection.Select(r => r.Follows));
		ActivityPublisherMock.Verify(m => m.Follow(target.Inbox, target, _profile));
		ActivityPublisherMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should add a new follower")]
	public async Task ReceiveFollowRequest()
	{
		var follower = new FakeProfile().Generate();
		DataAdapterMock.Setup(m => m.LookupProfile(_profile.FediId)).ReturnsAsync(_profile);
		DataAdapterMock.Setup(m => m.LookupProfile(follower.FediId)).ReturnsAsync(follower);

		var actual = await _service.ReceiveFollowRequest(_profile.FediId, follower.FediId, null);

		Assert.Equal(FollowState.Accepted, actual.State);
		Assert.Contains(follower, _profile.FollowersCollection.Select(r => r.Follower));
	}

	[Fact(DisplayName = "Should update a pending follow")]
	public async Task FollowReply()
	{
		var target = new FakeProfile().Generate();
		_profile.Follow(target, FollowState.Pending);
		DataAdapterMock.Setup(m => m.LookupProfileWithRelation(_profile.GetId(), target.FediId)).ReturnsAsync(_profile);

		var actual = await _service.ReceiveFollowReply(_profile.GetId(), target.FediId, FollowState.Accepted);

		Assert.Equal(FollowState.Accepted, actual.State);
		Assert.Equal(FollowState.Accepted,
			_profile.FollowingCollection.FirstOrDefault(r => r.Follows.FediId == target.FediId)?.State);
	}

	[Fact(DisplayName = "Should remove a pending follow on reject")]
	public async Task FollowReplyReject()
	{
		var target = new FakeProfile().Generate();
		_profile.Follow(target, FollowState.Pending);
		DataAdapterMock.Setup(m => m.LookupProfileWithRelation(_profile.GetId(), target.FediId)).ReturnsAsync(_profile);

		var actual = await _service.ReceiveFollowReply(_profile.GetId(), target.FediId, FollowState.Rejected);

		Assert.Equal(FollowState.Rejected, actual.State);
		Assert.DoesNotContain(target, _profile.FollowingCollection.Select(r => r.Follows));

		// Assert.Equal(FollowState.Accepted, _profile.Following.FirstOrDefault(r => r.Follows.Id == target.Id)?.State);
	}

	[InlineData(false)]
	[InlineData(true)]
	[Theory(DisplayName = "Should remove a follower")]
	public async Task RemoveFollower(bool useId)
	{
		var follower = new FakeProfile().Generate();
		_profile.AddFollower(follower, FollowState.Accepted);
		var queryable = new List<Profile> { _profile }.BuildMock();
		DataAdapterMock.Setup(m => m.SingleProfile(_profile.GetId())).Returns(queryable);
		DataAdapterMock.Setup(m => m.SingleProfile(_profile.FediId)).Returns(queryable);

		var actual = useId ? await _service.RemoveFollower(_profile.GetId(), follower.GetId())
			:await _service.RemoveFollower(_profile.GetId(), follower.FediId);

		Assert.DoesNotContain(follower, _profile.FollowersCollection.Select(r => r.Follower));
	}

	[InlineData(false)]
	[InlineData(true)]
	[Theory(DisplayName = "Should unfollow")]
	public async Task Unfollow(bool useId)
	{
		var follower = new FakeProfile().Generate();
		_profile.Follow(follower, FollowState.Accepted);
		var queryable = new List<Profile> { _profile }.BuildMock();
		DataAdapterMock.Setup(m => m.WithRelation(It.IsAny<IQueryable<Profile>>(), It.IsAny<Uuid7>()))
			.Returns(queryable);
		DataAdapterMock.Setup(m => m.WithRelation(It.IsAny<IQueryable<Profile>>(), It.IsAny<Uri>()))
			.Returns(queryable);

		var actual = useId ? await _service.Unfollow(_profile.GetId(), follower.GetId())
			: await _service.Unfollow(_profile.GetId(), follower.FediId);

		Assert.DoesNotContain(follower, _profile.FollowingCollection.Select(r => r.Follows));
	}

	[InlineData(false)]
	[InlineData(true)]
	[Theory(DisplayName = "Should accept a follower")]
	public async Task AcceptFollower(bool useId)
	{
		var follower = new FakeProfile().Generate();
		_profile.AddFollower(follower, FollowState.Pending);
		var queryable = new List<Profile> { _profile }.BuildMock();
		DataAdapterMock.Setup(m => m.WithRelation(It.IsAny<IQueryable<Profile>>(), It.IsAny<Uuid7>()))
			.Returns(queryable);
		DataAdapterMock.Setup(m => m.WithRelation(It.IsAny<IQueryable<Profile>>(), It.IsAny<Uri>()))
			.Returns(queryable);

		var actual = useId ? await _service.AcceptFollower(_profile.GetId(), follower.GetId())
			: await _service.AcceptFollower(_profile.GetId(), follower.FediId);

		Assert.Equal(FollowState.Accepted, actual.State);
		Assert.Contains(follower, _profile.FollowersCollection.Select(r => r.Follower));
	}

	[Fact(DisplayName = "Should not add a follower that did not request to follow")]
	public async Task NoForceFollower()
	{
		var follower = new FakeProfile().Generate();
		_profile.AddFollower(follower, FollowState.None);
		var queryable = new List<Profile> { _profile }.BuildMock();
		DataAdapterMock.Setup(m => m.WithRelation(It.IsAny<IQueryable<Profile>>(), It.IsAny<Uuid7>()))
			.Returns(queryable);

		var actual = await _service.AcceptFollower(_profile.GetId(), follower.GetId());

		Assert.Equal(FollowState.None, actual.State);
		Assert.Contains(follower, _profile.FollowersCollection.Select(r => r.Follower));
	}
}