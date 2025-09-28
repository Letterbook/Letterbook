using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.Core;

public class AccountService : IAccountService, IDisposable
{
	private readonly ILogger<AccountService> _logger;
	private readonly CoreOptions _opts;
	private readonly IDataAdapter _accountAdapter;
	private readonly IAccountEventPublisher _eventPublisherService;
	private readonly UserManager<Account> _identityManager;

	public AccountService(ILogger<AccountService> logger, IOptions<CoreOptions> options,
		IDataAdapter accountAdapter, IAccountEventPublisher eventPublisherService,
		UserManager<Account> identityManager)
	{
		_logger = logger;
		_opts = options.Value;
		_accountAdapter = accountAdapter;
		_eventPublisherService = eventPublisherService;
		_identityManager = identityManager;
	}

	public async Task<AccountIdentity> AuthenticatePassword(string email, string password)
	{
		var account = await _accountAdapter.FindAccountByEmail(_identityManager.NormalizeEmail(email));
		if (account == null) return AccountIdentity.Fail(false);
		if (account.LockoutEnd >= DateTime.UtcNow)
		{
			throw new RateLimitException("Too many failed attempts", account.LockoutEnd.GetValueOrDefault());
		}

		var match = _identityManager.PasswordHasher.VerifyHashedPassword(account,
			account.PasswordHash ?? string.Empty, password);
		switch (match)
		{
			case PasswordVerificationResult.SuccessRehashNeeded:
				await _identityManager.ResetAccessFailedCountAsync(account);
				account.PasswordHash = _identityManager.PasswordHasher.HashPassword(account, password);
				_accountAdapter.Update(account);
				await _accountAdapter.Commit();
				return AccountIdentity.Succeed(account.TwoFactorEnabled, account);
			case PasswordVerificationResult.Success:
				await _identityManager.ResetAccessFailedCountAsync(account);
				return AccountIdentity.Succeed(account.TwoFactorEnabled, account);
			case PasswordVerificationResult.Failed:
			default:
				await _identityManager.AccessFailedAsync(account);
				_logger.LogInformation("Password Authentication failed for {AccountId}", account.Id);
				return AccountIdentity.Fail(account.LockoutEnd > DateTimeOffset.UtcNow);
		}
	}

	public async Task<IdentityResult> ChangePassword(Guid id, string currentPassword, string newPassword)
	{
		if (await LookupAccount(id) is not { } account)
			throw CoreException.MissingData<Account>("Account not found", id);
		return await _identityManager.ChangePasswordAsync(account, currentPassword, newPassword);
	}

	public async Task<IdentityResult> RegisterAccount(string email, string handle, string password)
	{
		var baseUri = _opts.BaseUri();
		// TODO: write our own unified query for this
		if (await _identityManager.FindByNameAsync(handle) is not null)
			return IdentityResult.Failed(new IdentityError
			{
				Code = "Duplicate",
				Description = "An account with that username already exists"
			});
		if (await _identityManager.FindByEmailAsync(email) is not null)
			return IdentityResult.Failed(new IdentityError
			{
				Code = "Duplicate",
				Description = "An account is already registered using that email"
			});
		var account = Account.CreateAccount(baseUri, email, handle);

		IdentityResult created;
		try
		{
			created = await _identityManager.CreateAsync(account, password);
			if (!created.Succeeded) return created;
		}
		catch (Exception ex)
		{
			throw CoreException.InvalidRequest(ex.Message, innerEx: ex);
		}

		await _accountAdapter.Commit();
		_logger.LogInformation("Created new account {AccountId}", account.Id);
		await _eventPublisherService.Created(account);
		return created;
	}

	public async Task<Account?> LookupAccount(Guid id)
	{
		return await _accountAdapter
			.AllAccounts()
			.Where(account => account.Id == id)
			.WithProfiles()
			.FirstOrDefaultAsync();
	}

	public IAsyncEnumerable<Account> FindAccounts(string email)
	{
		return _accountAdapter.AllAccounts().WithProfiles()
			.Where(account => account.NormalizedEmail == _identityManager.NormalizeEmail(email))
			.AsAsyncEnumerable();
	}

	public async Task<Account?> FirstAccount(string email)
	{
		return await _accountAdapter.AllAccounts().WithProfiles()
			.Where(account => account.NormalizedEmail == _identityManager.NormalizeEmail(email))
			.FirstOrDefaultAsync();
	}

	public async Task<string> GenerateChangeEmailToken(Guid accountId, string email)
	{
		var account = await _accountAdapter.Accounts(accountId)
			.FirstOrDefaultAsync() ?? throw CoreException.MissingData<Account>(accountId);
		return await _identityManager.GenerateChangeEmailTokenAsync(account, email);
	}

	public async Task DeliverPasswordChangeLink(string email, string baseUrl)
	{
		var account = await _accountAdapter.AllAccounts().Where(a => a.NormalizedEmail == _identityManager.NormalizeEmail(email))
			.OrderBy(a => a.Email)
			.FirstOrDefaultAsync();
		if (account is null)
		{
			_logger.LogWarning("Attempted password reset for unknown account {Email}", email);
			return;
		}

		var token = await _identityManager.GeneratePasswordResetTokenAsync(account);
		var link = QueryHelpers.AddQueryString(baseUrl, "token", token);

		await _eventPublisherService.PasswordResetRequested(account, link);
	}

	public async Task<IdentityResult> ChangeEmailWithToken(string oldEmail, string newEmail, string token)
	{
		var account = await _accountAdapter.AllAccounts().Where(a => a.NormalizedEmail == _identityManager.NormalizeEmail(oldEmail))
			.OrderBy(a => a.NormalizedEmail)
			.FirstOrDefaultAsync() ?? throw CoreException.MissingData<Account>(oldEmail);
		var result = await _identityManager.ChangeEmailAsync(account, newEmail, token);
		return result;
	}

	public async Task<IdentityResult> ChangePasswordWithToken(string email, string password, string token)
	{
		var account = await _accountAdapter.AllAccounts().Where(a => a.NormalizedEmail == _identityManager.NormalizeEmail(email))
			.OrderBy(a => a.NormalizedEmail)
			.FirstOrDefaultAsync() ?? throw CoreException.MissingData<Account>(email);
		var result = await _identityManager.ResetPasswordAsync(account, token, password);
		return result;
	}

	public async Task<bool> AddLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims)
	{
		var account = await _accountAdapter.LookupAccount(accountId);
		if (account is null) return false;
		var count = account.LinkedProfiles.Count;
		var link = new ProfileClaims(account, profile, claims);
		account.LinkedProfiles.Add(link);
		await _accountAdapter.Commit();

		return count == account.LinkedProfiles.Count;
	}

	public async Task<bool> UpdateLinkedProfile(Guid accountId, Profile profile, IEnumerable<ProfileClaim> claims)
	{
		var account = await _accountAdapter.LookupAccount(accountId);
		if (account is null) return false;
		var model = new ProfileClaims(account, profile, [ProfileClaim.None]);
		var link = account.LinkedProfiles.SingleOrDefault(p => p.Equals(model));
		if (link is null)
		{
			_logger.LogWarning("Account {Account} has no link to Profile {Profile}", account.Id, profile.FediId);
			return false;
		}

		link.Claims = claims.ToList();

		await _accountAdapter.Commit();
		return true;
	}

	public async Task<bool> RemoveLinkedProfile(Guid accountId, Profile profile)
	{
		var account = await _accountAdapter.LookupAccount(accountId);
		if (account is null) return false;

		var link = account.LinkedProfiles.FirstOrDefault(claims => claims.Profile == profile);
		if (link is null) return false;

		var result = account.LinkedProfiles.Remove(link);
		if (result) await _accountAdapter.Commit();
		return result;
	}

	public async Task<IEnumerable<Claim>> LookupClaims(Account account)
	{
		return await _identityManager.GetClaimsAsync(account);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_identityManager.Dispose();
		_accountAdapter.Dispose();
	}
}