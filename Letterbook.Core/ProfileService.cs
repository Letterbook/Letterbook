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

    public ProfileService(ILogger<ProfileService> logger, IOptions<CoreOptions> options, IAccountProfileAdapter profiles)
    {
        _logger = logger;
        _options = options.Value;
        _profiles = profiles;
    }

    public Task<Profile> CreateProfile(Profile profile)
    {
        throw new NotImplementedException();
    }

    public async Task<Profile?> CreateProfile(Guid ownerId, string handle)
    {
        var account = _profiles.LookupAccount(ownerId);
        if (account == null)
        {
            _logger.LogError("Failed to create a new profile because no account exists with ID {AccountId}", ownerId);
            throw new InvalidException("Can't find account to attach to profile");
        }
        if (_profiles.SearchProfiles().Any(p => p.Handle == handle))
        {
            _logger.LogError("Cannot create a new profile because a profile already exists with handle {Handle}", handle);
            throw new DuplicateException("Profile already exists");
        }

        var profile = Profile.CreateIndividual(_options.BaseUri(), handle);
        profile.OwnedBy = account;
        account.LinkedProfiles.Add(new LinkedProfile(account, profile, ProfilePermission.All));
        _profiles.RecordAccount(account);
        await _profiles.Commit();

        return profile;
    }

    public Task<Profile> UpdateDisplayName(Guid localId, string displayName)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> UpdateDescription(Guid localId, string description)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> InsertCustomField(Guid localId, int index, string key, string value)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> RemoveCustomField(Guid localId, int index)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> UpdateCustomField(Guid localId, int index, string key, string value)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> UpdateProfile(Profile profile)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> LookupProfile(Guid localId)
    {
        throw new NotImplementedException();
    }

    public Task<Profile> LookupProfile(Uri id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Profile>> FindProfiles(string handle)
    {
        throw new NotImplementedException();
    }

    public Task<FollowResult> Follow(Guid selfId, Uri profileId, Uri? audienceId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowResult> Follow(Guid selfId, Guid localId, Uri? audienceId)
    {
        throw new NotImplementedException();
    }

    public Task<FollowResult> ReceiveFollower(Uri selfId, Uri followerId)
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
}