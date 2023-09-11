using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.Core;

public class AccountService : IAccountService, IDisposable
{
    private readonly ILogger<AccountService> _logger;
    private readonly CoreOptions _opts;
    private readonly IAccountProfileAdapter _accountAdapter;
    private readonly IAccountEventService _eventService;
    private readonly UserManager<Account> _identityManager;

    public AccountService(ILogger<AccountService> logger, IOptions<CoreOptions> options,
        IAccountProfileAdapter accountAdapter, IAccountEventService eventService,
        UserManager<Account> identityManager)
    {
        _logger = logger;
        _opts = options.Value;
        _accountAdapter = accountAdapter;
        _eventService = eventService;
        _identityManager = identityManager;
    }

    public async Task<IList<Claim>> AuthenticatePassword(string email, string password)
    {
        var accountAuth = await _identityManager.FindByEmailAsync(email);
        if (accountAuth == null) return Array.Empty<Claim>();
        if (accountAuth.LockoutEnd >= DateTime.UtcNow)
        {
            throw new RateLimitException("Too many failed attempts", accountAuth.LockoutEnd.GetValueOrDefault());
        }

        var match = _identityManager.PasswordHasher.VerifyHashedPassword(accountAuth,
            accountAuth.PasswordHash ?? string.Empty, password);
        switch (match)
        {
            case PasswordVerificationResult.SuccessRehashNeeded:
                await _identityManager.ResetAccessFailedCountAsync(accountAuth);
                accountAuth.PasswordHash = _identityManager.PasswordHasher.HashPassword(accountAuth, password);
                return await _identityManager.GetClaimsAsync(accountAuth);
            case PasswordVerificationResult.Success:
                await _identityManager.ResetAccessFailedCountAsync(accountAuth);
                return await _identityManager.GetClaimsAsync(accountAuth);
            case PasswordVerificationResult.Failed:
            default:
                await _identityManager.AccessFailedAsync(accountAuth);
                _logger.LogInformation("Password Authentication failed for {AccountId}", accountAuth.Id);
                return Array.Empty<Claim>();
        }
    }

    public async Task<Account?> RegisterAccount(string email, string handle, string password)
    {
        var baseUri = _opts.BaseUri();
        var account = Account.CreateAccount(baseUri, email, handle);
        var created = await _identityManager.CreateAsync(account, password);
        
        if (created.Succeeded && _accountAdapter.RecordAccount(account))
        {
            await _identityManager.AddClaimsAsync(account, new []
            {
                new Claim(JwtRegisteredClaimNames.Sub, handle),
                new Claim(JwtRegisteredClaimNames.Email, email)
            });
            await _accountAdapter.Commit();
            _logger.LogInformation("Created new account {AccountId}", account.Id);
            _eventService.Created(account);
            return account;
        }
        
        _logger.LogWarning("Could not create new account for {Email}", account.Email);
        return default;
    }

    public async Task<Account?> LookupAccount(Guid id)
    {
        return await _accountAdapter.LookupAccount(id);
    }

    public async Task<Profile> LookupProfile(string queryTarget)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Account>> FindAccounts(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateEmail(Guid accountId, string email)
    {
        var account = await _accountAdapter.LookupAccount(accountId);
        if (account == null) return false;
        account.Email = email;
        return true;
    }

    public async Task<bool> AddLinkedProfile(Guid accountId, Profile profile, ProfilePermission permission)
    {
        var account = await _accountAdapter.LookupAccount(accountId);
        if (account is null) return false;
        var count = account.LinkedProfiles.Count;
        var link = new LinkedProfile(account, profile, permission);
        profile.RelatedAccounts.Add(link);
        account.LinkedProfiles.Add(link);
        return count == account.LinkedProfiles.Count;
    }

    public async Task<bool> UpdateLinkedProfile(Guid accountId, Profile profile, ProfilePermission permission)
    {
        var account = await _accountAdapter.LookupAccount(accountId);
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

    public async Task<bool> RemoveLinkedProfile(Guid accountId, Profile profile)
    {
        var account = await _accountAdapter.LookupAccount(accountId);
        if (account is null) return false;

        var link = new LinkedProfile(account, profile, ProfilePermission.None);

        return profile.RelatedAccounts.Remove(link) && account.LinkedProfiles.Remove(link);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _identityManager.Dispose();
    }
}