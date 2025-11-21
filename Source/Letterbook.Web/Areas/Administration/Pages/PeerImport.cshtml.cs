using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class PeerImport : PageModel
{
	[BindProperty] public IFormFile CsvFile { get; set; } = default!;
	[BindProperty] public bool LetterbookFormat { get; set; }
	[BindProperty] public ModerationService.MergeStrategy Strategy { get; set; }

	public void OnGet()
	{

	}

	public string FormateStrategy(string name)
	{
		return string.Join(' ', StringFormatters.SplitExp().Split(name));
	}
}