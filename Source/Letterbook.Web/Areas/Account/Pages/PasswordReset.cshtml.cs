using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Account.Pages;

[AllowAnonymous]
public class PasswordReset : PageModel
{
	private readonly ILogger<PasswordReset> _logger;

	public string? Result { get; set; }

	public PasswordReset(ILogger<PasswordReset> logger)
	{
		_logger = logger;
	}

	public IActionResult OnGet()
	{
		return Page();
	}

	public async Task<IActionResult> OnPostReset(string email)
	{
		_logger.LogInformation("Requested password reset for {Email}", email);
		await Task.CompletedTask;
		return Page();
	}
}