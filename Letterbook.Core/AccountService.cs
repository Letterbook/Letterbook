using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly CoreOptions _opts;
    private readonly IAccountProfileAdapter _accountAdapter;
    private readonly IAccountEventService _eventService;

    public AccountService(ILogger<AccountService> logger, IOptions<CoreOptions> options,
        IAccountProfileAdapter accountAdapter, IAccountEventService eventService)
    {
        _logger = logger;
        _opts = options.Value;
        _accountAdapter = accountAdapter;
        _eventService = eventService;
    }

    public Account? RegisterAccount(string email, string handle)
    {
        // Fun fact, Uri will collapse the port number out of the string if it's the default for the scheme
        var baseUri = _opts.BaseUri();
        var account = Account.CreateAccount(baseUri, email, handle);

        var success = _accountAdapter.RecordAccount(account);
        if (success)
        {
            _logger.LogInformation("Created new account {AccountId}", account.Id);
            _eventService.Created(account);
            return account;
        }

        _logger.LogWarning("Could not create new account for {Email}", account.Email);
        return default;
    }

    public Account? LookupAccount(string id)
    {
        return _accountAdapter.LookupAccount(id);
    }

    public Profile LookupProfile(string queryTarget)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Account> FindAccounts(string email)
    {
        throw new NotImplementedException();
    }

    public bool UpdateEmail(string accountId, string email)
    {
        var account = _accountAdapter.LookupAccount(accountId);
        if (account == null) return false;
        account.Email = email;
        return true;
    }

    public bool AddLinkedProfile(string accountId, Profile profile, ProfilePermission permission)
    {
        var account = _accountAdapter.LookupAccount(accountId);
        if (account is null) return false;
        var count = account.LinkedProfiles.Count;
        var link = new LinkedProfile(account, profile, permission);
        profile.RelatedAccounts.Add(link);
        account.LinkedProfiles.Add(link);
        return count == account.LinkedProfiles.Count;
    }

    public bool UpdateLinkedProfile(string accountId, Profile profile, ProfilePermission permission)
    {
        var account = _accountAdapter.LookupAccount(accountId);
        if (account is null) return false;
        var model = new LinkedProfile(account, profile, ProfilePermission.None);
        var profileLink = profile.RelatedAccounts.SingleOrDefault(p => p.Equals(model));
        var accountLink = account.LinkedProfiles.SingleOrDefault(p => p.Equals(model));
        if (profileLink is null || accountLink is null)
        {
            _logger.LogWarning("Account {Account} and Profile {Profile} have mismatched permission links", account.Id,
                profile.Id);
            return false;
        }

        if (profileLink.Permission == permission || accountLink.Permission == permission) return false;

        profileLink.Permission = permission;
        accountLink.Permission = permission;
        return true;
    }

    public bool RemoveLinkedProfile(string accountId, Profile profile)
    {
        var account = _accountAdapter.LookupAccount(accountId);
        if (account is null) return false;

        var link = new LinkedProfile(account, profile, ProfilePermission.None);

        return profile.RelatedAccounts.Remove(link) && account.LinkedProfiles.Remove(link);
    }
}