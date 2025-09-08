using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

[Authorize(Policy = Constants.AuthzPolicy)]
public class Account : PageModel
{
	private readonly IAccountService _accounts;

	private Models.Account _self;

	public Guid AccountId => _self.Id;
	public string DisplayName => _self.Name ?? _self.UserName ?? "unknown";
	public string Email => _self.Email ?? "";
	public bool EmailConfirmed => _self.EmailConfirmed;

	public Account(IAccountService accounts)
	{
		_self = default!;
		_accounts = accounts;
	}

	public async Task<IActionResult> OnGet()
	{
		if (!User.Claims.TryGetAccountId(out var id) || await _accounts.LookupAccount(id) is not { } account)
			return Challenge();
		_self = account;

		return Page();
	}

}