using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Values;
using Medo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class ProfileService : IProfileService
{
	private ILogger<ProfileService> _logger;
	private CoreOptions _coreConfig;
	private IAccountProfileAdapter _profiles;
	private IProfileEventService _profileEvents;
	private readonly IActivityPubClient _client;

	public ProfileService(ILogger<ProfileService> logger, IOptions<CoreOptions> options,
		IAccountProfileAdapter profiles, IProfileEventService profileEvents, IActivityPubClient client)
	{
		_logger = logger;
		_coreConfig = options.Value;
		_profiles = profiles;
		_profileEvents = profileEvents;
		_client = client;
	}

	public Task<Profile> CreateProfile(Profile profile)
	{
		throw new NotImplementedException();
	}

	public async Task<Profile> CreateProfile(Guid ownerId, string handle)
	{
		var account = await _profiles.LookupAccount(ownerId);
		if (account == null)
		{
			_logger.LogError("Failed to create a new profile because no account exists with ID {AccountId}", ownerId);
			throw CoreException.MissingData("Cannot attach new Profile to Account because Account could not be found", typeof(Account), ownerId);
		}

		if (await _profiles.AnyProfile(handle))
		{
			_logger.LogError("Cannot create a new profile because a profile already exists with handle {Handle}",
				handle);
			throw CoreException.Duplicate("Profile already exists", handle);
		}

		var profile = Profile.CreateIndividual(_coreConfig.BaseUri(), handle);
		profile.OwnedBy = account;
		account.LinkedProfiles.Add(new ProfileAccess(account, profile, ProfilePermission.All));
		_profiles.RecordAccount(account);
		await _profiles.Commit();
		_profileEvents.Created(profile);

		return profile;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="localId"></param>
	/// <param name="displayName"></param>
	/// <returns>The original and updated Profiles, or null if no change was made</returns>
	/// <exception cref="CoreException"></exception>
	public async Task<UpdateResponse<Profile>> UpdateDisplayName(Guid localId, string displayName)
	{
		// TODO(moderation): vulgarity filters
		var profile = await RequireProfile(localId);
		if (profile.DisplayName == displayName) return new UpdateResponse<Profile>
		{
			Original = profile
		};

		var original = profile.ShallowClone();
		profile.DisplayName = displayName;
		await _profiles.Commit();
		_profileEvents.Updated(original: original, updated: profile);

		return new UpdateResponse<Profile>
		{
			Original = original,
			Updated = profile
		};
	}


	public async Task<UpdateResponse<Profile>> UpdateDescription(Guid localId, string description)
	{
		var profile = await RequireProfile(localId);
		if (profile.Description == description) return new UpdateResponse<Profile>
		{
			Original = profile
		};

		var original = profile.ShallowClone();
		profile.Description = description;
		await _profiles.Commit();
		_profileEvents.Updated(original: original, updated: profile);

		return new UpdateResponse<Profile>
		{
			Original = original,
			Updated = profile
		};
	}

	public async Task<UpdateResponse<Profile>> InsertCustomField(Guid localId, int index, string key,
		string value)
	{
		var profile = await RequireProfile(localId);
		if (profile.CustomFields.Length >= _coreConfig.MaxCustomFields)
			throw CoreException.InvalidRequest("Cannot add any more custom fields");
		var original = profile.ShallowClone();

		var customFields = profile.CustomFields.ToList();
		customFields.Insert(index, new CustomField { Label = key, Value = value });
		profile.CustomFields = customFields.ToArray();

		await _profiles.Commit();
		_profileEvents.Updated(original: original, updated: profile);

		return new UpdateResponse<Profile>
		{
			Original = original,
			Updated = profile
		};
	}

	public async Task<UpdateResponse<Profile>> RemoveCustomField(Guid localId, int index)
	{
		var profile = await RequireProfile(localId);
		if (index >= profile.CustomFields.Length)
			throw CoreException.InvalidRequest("Cannot remove custom field because it doesn't exist");
		var original = profile.ShallowClone();

		var customFields = profile.CustomFields.ToList();
		customFields.RemoveAt(index);
		profile.CustomFields = customFields.ToArray();

		await _profiles.Commit();
		_profileEvents.Updated(original: original, updated: profile);

		return new UpdateResponse<Profile>
		{
			Original = original,
			Updated = profile
		};
	}

	public async Task<UpdateResponse<Profile>> UpdateCustomField(Guid localId, int index, string key,
		string value)
	{
		var profile = await RequireProfile(localId);
		if (index >= profile.CustomFields.Length)
			throw CoreException.InvalidRequest("Cannot update custom field because it doesn't exist");
		var field = profile.CustomFields[index];
		if (field.Label == key && field.Value == value) return new UpdateResponse<Profile>
		{
			Original = profile
		};
		var original = profile.ShallowClone();

		profile.CustomFields[index] = new CustomField { Label = key, Value = value };

		await _profiles.Commit();
		_profileEvents.Updated(original: original, updated: profile);

		return new UpdateResponse<Profile>
		{
			Original = original,
			Updated = profile
		};
	}

	public Task<UpdateResponse<Profile>> UpdateProfile(Profile profile)
	{
		throw new NotImplementedException();
	}

	public async Task<Profile?> LookupProfile(Uuid7 localId)
	{
		return await _profiles.LookupProfile(localId);
	}

	public async Task<Profile?> LookupProfile(Uri id)
	{
		return await _profiles.LookupProfile(id);
	}

	public async Task<IEnumerable<Profile>> FindProfiles(string handle)
	{
		var results = _profiles.FindProfilesByHandle(handle);

		var profiles = new List<Profile>();
		await foreach (var profile in results)
		{
			profiles.Add(profile);
		}

		return profiles;
	}

	private async Task<FollowState> Follow(Profile self, Profile target, bool subscribeOnly)
	{
		if (target.Authority == _coreConfig.BaseUri().Authority)
		{
			// TODO(moderation): Check for blocks
			// TODO(moderation): Check for requiresApproval
			var relation = self.Follow(target, FollowState.Accepted);
			// target.FollowersCollection.Add(relation);
			self.Audiences.Add(subscribeOnly ? Audience.Subscribers(target) : Audience.Followers(target));
			self.Audiences.Add(Audience.Boosts(target));
			await _profiles.Commit();
			return relation.State;
		}

		// TODO(moderation): Check for blocks
		// TODO(moderation): Check for requiresApproval
		var followState = await _client.As(self).SendFollow(target.Inbox, target);
		switch (followState.Data)
		{
			case FollowState.Accepted:
			case FollowState.Pending:
				self.Follow(target, followState.Data);
				self.Audiences.Add(Audience.Followers(target));
				self.Audiences.Add(Audience.Boosts(target));
				await _profiles.Commit();
				return followState.Data;
			case FollowState.None:
			case FollowState.Rejected:
			default:
				return followState.Data;
		}
	}

	public async Task<FollowState> Follow(Guid selfId, Uri targetId)
	{
		var self = await RequireProfile(selfId, targetId);
		var target = await ResolveProfile(targetId, self);
		return await Follow(self, target, false);
	}

	public async Task<FollowState> Follow(Guid selfId, Guid targetId)
	{
		var target = await RequireProfile(targetId);
		var self = await RequireProfile(selfId, target.FediId);
		return await Follow(self, target, false);
	}

	public async Task<FollowerRelation> ReceiveFollowRequest(Uri targetId, Uri followerId, Uri? requestId)
	{
		if (targetId.Authority != _coreConfig.BaseUri().Authority)
		{
			_logger.LogWarning("Profile {FollowerId} tried to follow {TargetId}, but this is not the origin server", followerId, targetId);
			throw CoreException.WrongAuthority($"Cannot follow Profile {targetId} because it has a different origin server", targetId);
		}

		var target = await ResolveProfile(targetId);
		var follower = await ResolveProfile(followerId);

		return await ReceiveFollowRequest(target, follower, requestId);
	}

	public async Task<FollowerRelation> ReceiveFollowRequest(Guid localId, Uri followerId, Uri? requestId)
	{
		var target = await RequireProfile(localId);
		var follower = await ResolveProfile(followerId);

		return await ReceiveFollowRequest(target, follower, requestId);
	}

	private async Task<FollowerRelation> ReceiveFollowRequest(Profile target, Profile follower, Uri? requestId)
	{
		var relation = target.AddFollower(follower, FollowState.Accepted);
		await _profiles.Commit();
		// if (relation.State == FollowState.Rejected) return relation;

		// Todo: punt more AP responses to a delivery queue
		// Todo: also, implement that delivery queue
		// await _client.As(target).SendAccept(follower.Inbox, ActivityType.Follow, follower.Id, requestId);
		return relation;
	}

	public async Task<FollowState> ReceiveFollowReply(Uri selfId, Uri targetId, FollowState response)
	{
		if (selfId.Authority != _coreConfig.BaseUri().Authority)
		{
			_logger.LogWarning("Received a response to a follow request from {TargetId} concerning {SelfId}, but this is not the origin server", targetId, selfId);
			throw CoreException.WrongAuthority($"Cannot update Profile {selfId} because it has a different origin server", selfId);
		}
		var profile = await _profiles.LookupProfileWithRelation(selfId, targetId) ?? throw CoreException.MissingData($"Cannot update Profile {selfId} because it could not be found", typeof(Profile), selfId);
		var relation = profile.FollowingCollection.FirstOrDefault(r => r.Follows.FediId == targetId) ?? throw CoreException.MissingData($"Cannot update following relationship for {selfId} concerning {targetId} because it could not be found", typeof(FollowerRelation), targetId);
		switch (response)
		{
			case FollowState.Accepted:
			case FollowState.Pending:
				relation.State = response;
				await _profiles.Commit();
				return relation.State;
			case FollowState.None:
			case FollowState.Rejected:
			default:
				profile.Unfollow(relation.Follows);
				profile.LeaveAudience(relation.Follows);
				_profiles.Delete(relation);
				await _profiles.Commit();
				return FollowState.None;
		}
	}

	public async Task RemoveFollower(Guid selfId, Uri followerId)
	{
		var self = await _profiles.LookupProfileWithRelation(selfId, followerId)
				   ?? throw CoreException.MissingData(
					   $"Failed to update local profile {selfId} because it could not be found", typeof(Profile),
					   selfId);
		var follower = self.FollowersCollection
			.FirstOrDefault(p => p.Follower.FediId.OriginalString == followerId.OriginalString)?.Follower;

		if (follower is null) return;
		self.RemoveFollower(follower);
		await _profiles.Commit();
	}

	public async Task Unfollow(Guid selfId, Uri followerId)
	{
		var self = await _profiles.LookupProfileWithRelation(selfId, followerId)
				   ?? throw CoreException.MissingData(
					   $"Failed to update local profile {selfId} because it could not be found", typeof(Profile),
					   selfId);
		var follows = self.FollowingCollection
			.FirstOrDefault(p => p.Follows.FediId.OriginalString == followerId.OriginalString)?.Follows;

		if (follows is null) return;
		self.Unfollow(follows);
		await _profiles.Commit();
	}

	public Task ReportProfile(Guid selfId, Uri profileId)
	{
		throw new NotImplementedException();
	}

	public Task ReportProfile(Guid selfId, Guid localId)
	{
		throw new NotImplementedException();
	}

	[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
	private async Task<Profile> RequireProfile(Guid localId, Uri? relationId = null,
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
	{
		var profile = relationId != null
			? await _profiles.LookupProfileWithRelation(localId, relationId)
			: await _profiles.LookupProfile(localId);
		if (profile != null) return profile;

		_logger.LogError("Cannot update Profile {ProfileId} because it could not be found", localId);
		await _profiles.Cancel();
		throw CoreException.MissingData("Failed to update Profile because it could not be found", typeof(Profile), localId,
			null, name, path, line);
	}

	private async Task<Profile> ResolveProfile(Uri profileId,
		Profile? onBehalfOf = null,
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
	{
		var profile = await _profiles.LookupProfile(profileId);
		if (profile != null
			&& !profile.HasLocalAuthority(_coreConfig)
			&& profile.Updated.Add(TimeSpan.FromHours(12)) >= DateTime.UtcNow) return profile;

		try
		{
			if (profile != null)
			{
				var fetched = await _client.As(onBehalfOf).Fetch<Profile>(profileId);
				profile.ShallowCopy(fetched);
				_profiles.Update(profile);
			}
			else
			{
				profile = await _client.As(onBehalfOf).Fetch<Profile>(profileId);
				_profiles.Add(profile);
			}

			await _profiles.Commit();
		}
		catch (AdapterException)
		{
			_logger.LogError("Cannot resolve Profile {ProfileId}", profileId);
			await _profiles.Cancel();
			throw;
		}
		_logger.LogInformation("Fetched Profile {ProfileId} from origin", profileId);
		return profile;

	}
}