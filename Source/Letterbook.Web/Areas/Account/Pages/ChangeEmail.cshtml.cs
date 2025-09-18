using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Account.Pages;

public class ChangeEmail : PageModel
{
	private readonly ILogger<ChangeEmail> _logger;
	public required string ChangeEmailResult { get; set; }
	public required string ChangeEmailDetail { get; set; }

	public ChangeEmail(ILogger<ChangeEmail> logger)
	{
		_logger = logger;
	}

	public async Task<IActionResult> OnGet([FromQuery] string token, [FromQuery] string oldEmail, [FromQuery] string newEmail)
	{
		_logger.LogInformation("ChangeEmail {OldEmail} to {NewEmail} authorized by {Token}", oldEmail, newEmail, token);
		await Task.CompletedTask;

		ChangeEmailResult = "Not Implemented";
		ChangeEmailDetail = "But soon. Soon.";

		return Page();
	}
}