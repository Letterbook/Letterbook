using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class Account : PageModel
{
	public Guid AccountId => Guid.Empty;
	public string DisplayName => "User";
	public string Email => "user@example.com";

	public void OnGet()
	{

	}
}