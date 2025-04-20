using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Medo;
using MockQueryable;

namespace Letterbook.Web.Mocks;

public class MockProfileService : IProfileService, IAuthzProfileService
{
	private ProfileService _profileService;
	private FakeProfile _profiles;

	public MockProfileService(ProfileService profileService)
	{
		_profileService = profileService;
		_profiles = new FakeProfile();
	}

	public IAuthzProfileService As(IEnumerable<Claim> claims)
	{
		_profileService.As(claims);
		return this;
	}

	public Task<Models.Profile> CreateProfile(Models.Profile profile)
	{
		return _profileService.CreateProfile(profile);
	}
	public Task<Models.Profile> CreateProfile(Uuid7 ownerId, string handle)
	{
		return _profileService.CreateProfile(ownerId, handle);
	}
	public Task<Models.UpdateResponse<Models.Profile>> UpdateDisplayName(Models.ProfileId localId, string displayName)
	{
		return _profileService.UpdateDisplayName(localId, displayName);
	}
	public Task<Models.UpdateResponse<Models.Profile>> UpdateDescription(Models.ProfileId localId, string description)
	{
		return _profileService.UpdateDescription(localId, description);
	}
	public Task<Models.UpdateResponse<Models.Profile>> InsertCustomField(Models.ProfileId localId, int index, string key, string value)
	{
		return _profileService.InsertCustomField(localId, index, key, value);
	}
	public Task<Models.UpdateResponse<Models.Profile>> RemoveCustomField(Models.ProfileId localId, int index)
	{
		return _profileService.RemoveCustomField(localId, index);
	}
	public Task<Models.UpdateResponse<Models.Profile>> UpdateCustomField(Models.ProfileId localId, int index, string key, string value)
	{
		return _profileService.UpdateCustomField(localId, index, key, value);
	}
	public Task<Models.UpdateResponse<Models.Profile>> UpdateProfile(Models.Profile profile)
	{
		return _profileService.UpdateProfile(profile);
	}
	public Task<Models.Profile?> LookupProfile(Models.ProfileId profileId, Models.ProfileId? relatedProfile = null)
	{
		return _profileService.LookupProfile(profileId, relatedProfile);
	}
	public Task<Models.Profile?> LookupProfile(Uri fediId, Models.ProfileId? relatedProfile = null)
	{
		return _profileService.LookupProfile(fediId, relatedProfile);
	}
	public IAsyncEnumerable<Models.Profile> FindProfiles(string handle, string host)
	{
		return _profileService.FindProfiles(handle, host);
	}
	public IAsyncEnumerable<Models.Profile> FindProfiles(string handle)
	{
		return _profileService.FindProfiles(handle);
	}
	public IQueryable<Models.Profile> QueryProfiles(string handle, string host)
	{
		return host switch
		{
			"mock" => Mock(),
			_ => _profileService.QueryProfiles(handle, host)
		};

		IQueryable<Models.Profile> Mock()
		{
			var p = _profiles.Generate();
			p.Handle = handle;
			var posts = new FakePost(p).Generate(1300);
			foreach (var post in posts)
			{
				p.Posts.Add(post);
			}
			return new List<Models.Profile> { p }.BuildMock();
		}
	}
	public IQueryable<Models.Profile> QueryProfiles(string handle)
	{
		return _profileService.QueryProfiles(handle);
	}
	public IQueryable<Models.Profile> QueryProfiles(Models.ProfileId id)
	{
		return _profileService.QueryProfiles(id);
	}
	public Task<Models.FollowerRelation> Follow(Models.ProfileId selfId, Uri targetId)
	{
		return _profileService.Follow(selfId, targetId);
	}
	public Task<Models.FollowerRelation> Follow(Models.ProfileId selfId, Models.ProfileId targetId)
	{
		return _profileService.Follow(selfId, targetId);
	}
	public Task<Models.FollowerRelation> ReceiveFollowRequest(Uri targetId, Uri followerId, Uri? requestId)
	{
		return _profileService.ReceiveFollowRequest(targetId, followerId, requestId);
	}
	public Task<Models.FollowerRelation> ReceiveFollowRequest(Models.ProfileId localId, Uri followerId, Uri? requestId)
	{
		return _profileService.ReceiveFollowRequest(localId, followerId, requestId);
	}
	public Task<Models.FollowerRelation> ReceiveFollowReply(Models.ProfileId selfId, Uri targetId, FollowState response)
	{
		return _profileService.ReceiveFollowReply(selfId, targetId, response);
	}
	public Task<Models.FollowerRelation> RemoveFollower(Models.ProfileId selfId, Uri followerId)
	{
		return _profileService.RemoveFollower(selfId, followerId);
	}
	public Task<Models.FollowerRelation> RemoveFollower(Models.ProfileId selfId, Models.ProfileId followerId)
	{
		return _profileService.RemoveFollower(selfId, followerId);
	}
	public Task<Models.FollowerRelation> Unfollow(Models.ProfileId selfId, Uri targetId)
	{
		return _profileService.Unfollow(selfId, targetId);
	}
	public Task<Models.FollowerRelation?> Unfollow(Models.ProfileId selfId, Models.ProfileId targetId)
	{
		return _profileService.Unfollow(selfId, targetId);
	}
	public Task<IQueryable<Models.Profile>> LookupFollowing(Models.ProfileId profileId, DateTimeOffset? followedBefore, int limit)
	{
		return _profileService.LookupFollowing(profileId, followedBefore, limit);
	}
	public Task<IQueryable<Models.Profile>> LookupFollowers(Models.ProfileId profileId, DateTimeOffset? followedBefore, int limit)
	{
		return _profileService.LookupFollowers(profileId, followedBefore, limit);
	}
	public Task<Models.FollowerRelation> AcceptFollower(Models.ProfileId profileId, Models.ProfileId followerId)
	{
		return _profileService.AcceptFollower(profileId, followerId);
	}
	public Task<Models.FollowerRelation> AcceptFollower(Models.ProfileId profileId, Uri followerId)
	{
		return _profileService.AcceptFollower(profileId, followerId);
	}
	public Task<int> FollowerCount(Models.Profile profile)
	{
		return _profileService.FollowerCount(profile);
	}
	public Task<int> FollowingCount(Models.Profile profile)
	{
		return _profileService.FollowingCount(profile);
	}
	public Task ReportProfile(Uuid7 selfId, Uri profileId)
	{
		return _profileService.ReportProfile(selfId, profileId);
	}
	public Task<Models.FollowerRelation> Block(Models.ProfileId self, Models.ProfileId target)
	{
		return _profileService.Block(self, target);
	}
	public Task<Models.FollowerRelation> Unblock(Models.ProfileId self, Models.ProfileId target)
	{
		return _profileService.Unblock(self, target);
	}
	public Task<Models.FollowerRelation?> ReceiveBlock(Uri actorId, Uri targetId)
	{
		return _profileService.ReceiveBlock(actorId, targetId);
	}
	public Task<Models.FollowerRelation?> ReceiveUndoBlock(Uri actorId, Uri targetId)
	{
		return _profileService.ReceiveUndoBlock(actorId, targetId);
	}
}