using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class AboutModel : PageModel
{
	private readonly ILogger<AboutModel> _logger;

	public AboutModel(ILogger<AboutModel> logger)
	{
		_logger = logger;
	}

	public void OnGet()
	{
	}
}