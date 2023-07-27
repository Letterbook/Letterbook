using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IAccountAdapter _adapter;
    private readonly CoreOptions _opts;

    public AccountService(ILogger<AccountService> logger, IOptions<CoreOptions> options, IAccountAdapter adapter)
    {
        _logger = logger;
        _adapter = adapter;
        _opts = options.Value;
    }

    public Account? RegisterAccount(string email, string username)
    {
        // TODO: profile uri format
        // Fun fact, Uri will collapse the port number out of the string if it's the default for the scheme
        var profileId = new Uri($"{_opts.Scheme}://{_opts.DomainName}:{_opts.Port}/actor/@{username}");
        var account = Account.CreateAccount(email, profileId);

        var success = _adapter.RecordAccount(account);
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
        throw new NotImplementedException();
    }

    public IEnumerable<Account> FindAccounts(string email)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Account> FindRelatedAccounts(Profile profile)
    {
        throw new NotImplementedException();
    }

    public bool UpdateAccount(Account account)
    {
        throw new NotImplementedException();
    }
}