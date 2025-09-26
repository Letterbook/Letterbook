using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

[Authorize(Policy = Constants.AuthzPolicy)]
public class Account : PageModel
{
	internal const string ConfirmEmailAction = "ConfirmEmail";
	internal const string ChangeEmailAction = "ChangeEmail";
	internal const string ResetPasswordAction = "ResetPassword";

	private readonly IAccountService _accounts;
	private readonly ILogger<Account> _logger;

	private Models.Account _self;

	[FromQuery(Name = "PageAction")] public string? PageAction { get; set; }

	public Guid AccountId => _self.Id;
	public string DisplayName => _self.Name ?? _self.UserName ?? "unknown";
	public string Email => _self.Email ?? "";
	public bool EmailConfirmed => _self.EmailConfirmed;
	public string? ConfirmUrl { get; set; }

	public string ConfirmEmailResult { get; set; } = "";
	public List<string> ConfirmEmailDetails { get; set; } = [];

	public Account(IAccountService accounts, ILogger<Account> logger)
	{
		_self = default!;
		_accounts = accounts;
		_logger = logger;
	}

	public async Task<IActionResult> OnGet([FromQuery] string? token = null, [FromQuery] string? oldEmail = null,
		[FromQuery] string? newEmail = null)
	{
		if (!User.Claims.TryGetAccountId(out var id) || await _accounts.LookupAccount(id) is not { } account)
			return Challenge();
		_self = account;

		switch (PageAction)
		{
			case ConfirmEmailAction:
				if (token is null || oldEmail is null || newEmail is null)
					return BadRequest();
				await ActionConfirmEmail(token, oldEmail, newEmail);
				break;
			case ChangeEmailAction:
				break;
		}

		return Page();
	}

	public async Task<IActionResult> OnPostEmail(string newEmail, string oldEmail)
	{
		if (!User.Claims.TryGetAccountId(out var id))
			return Challenge();

		var token = await _accounts.GenerateChangeEmailToken(id, newEmail);
		ConfirmUrl = Url.PageLink(nameof(Account), values: new { PageAction = "ConfirmEmail", token, oldEmail, newEmail });

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
		if (!result.Succeeded)
			return BadRequest();

		return RedirectToPage();
	}

	private async Task ActionConfirmEmail(string token, string oldEmail, string newEmail)
	{
		var result = await _accounts.ChangeEmailWithToken(oldEmail, newEmail, token);

		if (result.Succeeded)
		{
			ConfirmEmailResult = "Success";
			ConfirmEmailDetails = ["Your email address has been changed"];
		}
		else
		{
			ConfirmEmailDetails = result.Errors.Select(e => e.Description).ToList();
			ConfirmEmailResult = "Your email address has not been changed";
		}
	}
}