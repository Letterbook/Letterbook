using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IAccountProfileAdapter _accountAdapter;
    private readonly CoreOptions _opts;

    public AccountService(ILogger<AccountService> logger, IOptions<CoreOptions> options, IAccountProfileAdapter accountAdapter)
    {
        _logger = logger;
        _accountAdapter = accountAdapter;
        _opts = options.Value;
    }

    public Account? RegisterAccount(string email, string handle)
    {
        // Fun fact, Uri will collapse the port number out of the string if it's the default for the scheme
        var baseUri = new Uri($"{_opts.Scheme}://{_opts.DomainName}:{_opts.Port}");
        var account = Account.CreateAccount(baseUri, email, handle);

        var success = _accountAdapter.RecordAccount(account);
        if (success)
        {
            _logger.LogInformation("Created new account {AccountId}", account.Id);
            return account;
        }

        _logger.LogWarning("Could not create new account for {Email}", account.Email);
        return default;
    }

    public Account? LookupAccount(string id)
    {
        return _accountAdapter.LookupAccount(id);
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
        var count = profile.RelatedAccounts.Count;
        profile.RelatedAccounts.Add(new LinkedProfile(account, profile, permission));
        return count == profile.RelatedAccounts.Count;
    }

    public bool UpdateLinkedProfile(string accountId, Profile profile, ProfilePermission permission)
    {
        var account = _accountAdapter.LookupAccount(accountId);
        if (account is null) return false;
        var model = new LinkedProfile(account, profile, ProfilePermission.None);
        var linkedProfile = profile.RelatedAccounts.SingleOrDefault(p => p.Equals(model));
        if (linkedProfile is null) return false;
        if (linkedProfile.Permission == permission) return false;
        
        linkedProfile.Permission = permission;
        return true;
    }

    public bool RemoveLinkedProfile(string accountId, Profile profile)
    {
        var account = _accountAdapter.LookupAccount(accountId);
        if (account is null) return false;

        return profile.RelatedAccounts.Remove(new LinkedProfile(account, profile, ProfilePermission.None));
    }
}