using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class PeerImport : PageModel
{
	[BindProperty] public IFormFile CsvFile { get; set; } = default!;
	[BindProperty] public bool LetterbookFormat { get; set; }

	public void OnGet()
	{

	}
}