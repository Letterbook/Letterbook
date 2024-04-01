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
				_accountAdapter.Update(accountAuth);
				await _accountAdapter.Commit();
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

			await _identityManager.AddClaimsAsync(account, new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, email)
			});
		}
		catch (Exception ex)
		{
			throw CoreException.InvalidRequest(ex.Message, innerEx: ex);
		}

		await _accountAdapter.Commit();
		_logger.LogInformation("Created new account {AccountId}", account.Id);
		_eventService.Created(account);
		return created;
	}

	public async Task<Account?> LookupAccount(Guid id)
	{
		return await _accountAdapter.LookupAccount(id);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<IEnumerable<Account>> FindAccounts(string email)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		throw new NotImplementedException();
	}

	// TODO: do this through Identity
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
		var link = new ProfileAccess(account, profile, permission);
		account.LinkedProfiles.Add(link);
		await _accountAdapter.Commit();

		return count == account.LinkedProfiles.Count;
	}

	public async Task<bool> UpdateLinkedProfile(Guid accountId, Profile profile, ProfilePermission permission)
	{
		var account = await _accountAdapter.LookupAccount(accountId);
		if (account is null) return false;
		var model = new ProfileAccess(account, profile, ProfilePermission.None);
		var link = account.LinkedProfiles.SingleOrDefault(p => p.Equals(model));
		if (link is null)
		{
			_logger.LogWarning("Account {Account} has no link to Profile {Profile}", account.Id, profile.FediId);
			return false;
		}

		if (link.Permission == permission) return false;

		link.Permission = permission;

		await _accountAdapter.Commit();
		return true;
	}

	public async Task<bool> RemoveLinkedProfile(Guid accountId, Profile profile)
	{
		var account = await _accountAdapter.LookupAccount(accountId);
		if (account is null) return false;

		var link = new ProfileAccess(account, profile, ProfilePermission.None);

		var result = account.LinkedProfiles.Remove(link);
		if (result) await _accountAdapter.Commit();
		return result;
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_identityManager.Dispose();
		_accountAdapter.Dispose();
	}
}