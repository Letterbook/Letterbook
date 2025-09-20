using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Account.Pages;

public class ChangeEmail : PageModel
{
	private readonly ILogger<ChangeEmail> _logger;
	private readonly IAccountService _accounts;

	public required string ChangeEmailResult { get; set; }
	public required List<string> ChangeEmailDetails { get; set; }

	public ChangeEmail(ILogger<ChangeEmail> logger, IAccountService accounts)
	{
		_logger = logger;
		_accounts = accounts;
	}

	public async Task<IActionResult> OnGet([FromQuery] string token, [FromQuery] string oldEmail, [FromQuery] string newEmail)
	{
		_logger.LogInformation("ChangeEmail {OldEmail} to {NewEmail} authorized by {Token}", oldEmail, newEmail, token);
		var result = await _accounts.ChangeEmailWithToken(oldEmail, newEmail, token);

		if (result.Succeeded)
		{
			ChangeEmailResult = "Success";
			ChangeEmailDetails = ["Your email address has been changed"];
		}

		ChangeEmailDetails = result.Errors.Select(e => e.Description).ToList();
		ChangeEmailResult = "Your email address has not been changed";

		return Page();
	}
}