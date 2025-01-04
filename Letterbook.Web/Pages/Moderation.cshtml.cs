using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class ModerationModel : PageModel
{
	private readonly ILogger<ModerationModel> _logger;

	public ModerationModel(ILogger<ModerationModel> logger)
	{
		_logger = logger;
	}

	public void OnGet()
	{
	}
}