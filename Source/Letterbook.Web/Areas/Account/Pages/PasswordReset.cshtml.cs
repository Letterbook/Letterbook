using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Account.Pages;

public class PasswordReset : PageModel
{
	private readonly IAccountService _accounts;
	public required string Token { get; set; }
	public required string Email { get; set; }
	public string? Result { get; set; }
	public List<string> ResultDetails { get; set; } = [];

	public PasswordReset(IAccountService accounts)
	{
		_accounts = accounts;
	}

	public IActionResult OnGet([FromQuery] string? email, [FromQuery] string? token)
	{
		if (email is null || token is null)
			return BadRequest();

		Email = email;
		Token = token;
		return Page();
	}

	public async Task<IActionResult> OnPost([FromForm] string? email, [FromForm] string? token, [FromForm] string passwordNew, [FromForm] string passwordConfirm)
	{
		if (email is null || token is null)
			return BadRequest();

		Email = email;
		Token = token;

		if (await _accounts.FirstAccount(email) is not { } account)
			return NotFound();

		if (passwordNew != passwordConfirm)
		{
			Result = "Failed";
			ResultDetails.Add("The new password and the confirmation password did not match");
			return Page();
		}

		var result = await _accounts.ChangePasswordWithToken(email, passwordNew, token);
		if (result.Succeeded)
		{
			Result = "Success";
			ResultDetails.Add("Your password has been changed");
		}
		else
		{
			Result = "Failed";
			ResultDetails.AddRange(result.Errors.Select(e => e.Description));
		}

		return Page();
	}
}