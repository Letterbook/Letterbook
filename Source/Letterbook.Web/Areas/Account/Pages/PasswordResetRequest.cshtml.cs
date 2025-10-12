using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Account.Pages;

[AllowAnonymous]
public class PasswordResetRequest : PageModel
{
	private readonly ILogger<PasswordResetRequest> _logger;
	private readonly IAccountService _accounts;

	public string? Result { get; set; }

	public PasswordResetRequest(ILogger<PasswordResetRequest> logger, IAccountService accounts)
	{
		_logger = logger;
		_accounts = accounts;
	}

	public IActionResult OnGet()
	{
		return Page();
	}

	public async Task<IActionResult> OnPostReset(string email)
	{
		var template = Url.PageLink(nameof(PasswordReset), null, new {area = "Account", email})
		               ?? throw CoreException.InternalError("Failed to generate a password reset link template");
		await _accounts.DeliverPasswordResetLink(email, template);
		_logger.LogInformation("Requested password reset for {Email}", email);

		Result = "Received";

		return Page();
	}
}