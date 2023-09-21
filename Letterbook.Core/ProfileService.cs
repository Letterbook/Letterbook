using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Values;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stateless;

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
            throw CoreException.Invalid("Cannot attach new Profile to Account because Account could not be found",
                "AccountId", ownerId);
        }

        if (await _profiles.AnyProfile(p => p.Handle == handle))
        {
            _logger.LogError("Cannot create a new profile because a profile already exists with handle {Handle}",
                handle);
            throw CoreException.Duplicate("Profile already exists", handle);
        }

        var profile = Profile.CreateIndividual(_coreConfig.BaseUri(), handle);
        profile.OwnedBy = account;
        account.LinkedProfiles.Add(new LinkedProfile(account, profile, ProfilePermission.All));
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
        // TODO: vulgarity filters
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
            throw CoreException.Invalid("Cannot add any more custom fields");
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
            throw CoreException.Invalid("Cannot remove custom field because it doesn't exist");
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
            throw CoreException.Invalid("Cannot update custom field because it doesn't exist");
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

    public async Task<Profile?> LookupProfile(Guid localId)
    {
        return await _profiles.LookupProfile(localId);
    }

    public async Task<Profile?> LookupProfile(Uri id)
    {
        return await _profiles.LookupProfile(id);
    }

    public async Task<IEnumerable<Profile>> FindProfiles(string handle)
    {
        var results = _profiles.QueryProfiles(source => source
            .Where(p => p.Handle == handle)
            .OrderBy(p => p.Handle));

        var profiles = new List<Profile>();
        await foreach (var profile in results)
        {
            profiles.Add(profile);
        }

        return profiles;
    }

    private async Task<FollowState> Follow(Profile self, Profile target, bool subscribeOnly,
        IEnumerable<Uri> additionalAudience)
    {
        // follow is local
        if (target.Authority == _coreConfig.BaseUri().Authority)
        {
            // TODO: Check for blocks
            // TODO: Check for requiresApproval
            self.AddFollowing(target, FollowState.Accepted);
            target.AddFollower(self, FollowState.Accepted);
            self.Audiences.Add(subscribeOnly ? Audience.Subscribers(target) : Audience.Followers(target));
            self.Audiences.Add(Audience.Boosts(target));
            foreach (var uri in additionalAudience)
            {
                self.Audiences.Add(Audience.FromUri(uri));
            }
            await _profiles.Commit();
            return FollowState.Accepted;
        }
        else
        {
            // TODO: Check for blocks
            // TODO: Check for requiresApproval

            var result = await _client.As(self)
                .SendFollow(target.Inbox, new FollowerRelation(self, target, FollowState.Pending));
            // result == result.Rejected ? return : self.Following.Add(new FollowerRelation(self, target, result));
            // etc
            throw new NotImplementedException();
        }
    }

    public async Task<FollowState> Follow(Guid selfId, Uri targetId, Uri? audienceId)
    {
        var self = await RequireProfile(selfId);
        var target = await ResolveProfile(self, targetId);
        return await Follow(self, target, false, audienceId is null ? Array.Empty<Uri>() : new []{audienceId});
    }

    public async Task<FollowState> Follow(Guid selfId, Guid targetId, Uri? audienceId)
    {
        var self = await RequireProfile(selfId);
        var target = await RequireProfile(targetId);
        return await Follow(self, target, false, audienceId is null ? Array.Empty<Uri>() : new []{audienceId});
    }

    public Task<FollowState> ReceiveFollowRequest(Uri selfId, Uri followerId)
    {
        // doesn't confirm, just do the follow
        // does confirm, mark pending
        throw new NotImplementedException();
    }

    public Task<FollowState> ReceiveFollowResponse(Uri selfId, Uri followerId, FollowState response)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFollower(Guid selfId, Uri followerId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFollower(Guid selfId, Guid followerId)
    {
        throw new NotImplementedException();
    }

    public Task Unfollow(Guid selfId, Uri followerId)
    {
        throw new NotImplementedException();
    }

    public Task Unfollow(Guid selfId, Guid followerId)
    {
        throw new NotImplementedException();
    }

    public Task ReportProfile(Guid selfId, Uri profileId)
    {
        throw new NotImplementedException();
    }

    public Task ReportProfile(Guid selfId, Guid localId)
    {
        throw new NotImplementedException();
    }

    private async Task<Profile> RequireProfile(Guid localId,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var profile = await _profiles.LookupProfile(localId);
        if (profile != null) return profile;

        _logger.LogError("Cannot update Profile {ProfileId} because it could not be found", localId);
        await _profiles.Cancel();
        // ReSharper disable ExplicitCallerInfoArgument Pass original call site details for better error reporting
        throw CoreException.Invalid("Failed to update Profile because it could not be found", "ProfileId", localId,
            null, name, path, line);
        // ReSharper restore ExplicitCallerInfoArgument
    }
    
    private async Task<Profile> ResolveProfile(Profile onBehalfOf, Uri profileId,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var profile = await _profiles.LookupProfile(profileId);
        if (profile != null && profile.Updated.Add(TimeSpan.FromHours(12)) >= DateTime.UtcNow) return profile;

        try
        {
            profile = await _client.As(onBehalfOf).Fetch<Profile>(profileId);
        }
        catch (AdapterException)
        {
            _logger.LogError("Cannot resolve Profile {ProfileId}", profileId);
            await _profiles.Cancel();
            throw;
        }
        _profiles.RecordProfile(profile);
        _logger.LogInformation("Fetched Profile {ProfileId} from origin", profileId);
        return profile;
        
    }
}