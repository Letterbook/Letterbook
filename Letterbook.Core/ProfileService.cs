using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Values;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class ProfileService : IProfileService
{
    private ILogger<ProfileService> _logger;
    private CoreOptions _options;
    private IAccountProfileAdapter _profiles;
    private IProfileEventService _profileEvents;

    public ProfileService(ILogger<ProfileService> logger, IOptions<CoreOptions> options,
        IAccountProfileAdapter profiles, IProfileEventService profileEvents)
    {
        _logger = logger;
        _options = options.Value;
        _profiles = profiles;
        _profileEvents = profileEvents;
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

        var profile = Profile.CreateIndividual(_options.BaseUri(), handle);
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
        if (profile.CustomFields.Length >= _options.MaxCustomFields)
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

    public async Task<FollowResult> Follow(Guid selfId, Uri profileId, Uri? audienceId)
    {
        var self = await _profiles.LookupProfile(selfId);
        var profile = await _profiles.LookupProfile(profileId);
        
        // state machine?????
        
        // follow is remote
        throw new NotImplementedException();
    }

    public Task<FollowResult> Follow(Guid selfId, Guid localId, Uri? audienceId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowResult> ReceiveFollowRequest(Uri selfId, Uri followerId)
    {
        // doesn't confirm, just do the follow
        // does confirm, mark pending
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

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    private async Task<Profile> RequireProfile(Guid localId,
        [CallerMemberName] string name = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int line = -1)
    {
        var profile = await _profiles.LookupProfile(localId);
        if (profile != null) return profile;
        _logger.LogError("Cannot update Profile {ProfileId} because it could not be found", localId);
        throw CoreException.Invalid("Failed to update Profile because it could not be found", "ProfileId", localId,
            name, path, line);
    }
}