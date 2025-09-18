using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Web.Areas.Account;
using Letterbook.Web.Areas.Account.Pages;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

[Authorize(Policy = Constants.AuthzPolicy)]
public class Account : PageModel
{
	private readonly IAccountService _accounts;
	private readonly ILogger<Account> _logger;

	private Models.Account _self;

	public Guid AccountId => _self.Id;
	public string DisplayName => _self.Name ?? _self.UserName ?? "unknown";
	public string Email => _self.Email ?? "";
	public bool EmailConfirmed => _self.EmailConfirmed;
	public string? ConfirmUrl { get; set; }


	public Account(IAccountService accounts, ILogger<Account> logger)
	{
		_self = default!;
		_accounts = accounts;
		_logger = logger;
	}

	public async Task<IActionResult> OnGet()
	{
		if (!User.Claims.TryGetAccountId(out var id) || await _accounts.LookupAccount(id) is not { } account)
			return Challenge();
		_self = account;

		return Page();
	}

	public async Task<IActionResult> OnPostEmail(string newEmail, string oldEmail)
	{
		if (!User.Claims.TryGetAccountId(out var id))
			return Challenge();

		var token = await _accounts.GenerateChangeEmailToken(id, newEmail);
		ConfirmUrl = Url.PageLink(nameof(ChangeEmail), values: new { area = "Account", token, oldEmail, newEmail });

		if (await _accounts.LookupAccount(id) is not { } account)
			return Challenge();
		_self = account;

		return Page();
	}

	public async Task<IActionResult> OnPostPassword(string passwordCurrent, string passwordNew, string passwordConfirm)
	{
		if (!User.Claims.TryGetAccountId(out var id))
			return Challenge();

		if (passwordNew != passwordConfirm)
			return BadRequest();

		var result = await _accounts.ChangePassword(id, passwordCurrent, passwordNew);
		if(!result.Succeeded)
			return BadRequest();

		return RedirectToPage();
	}

}