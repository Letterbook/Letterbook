using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class Peer : PageModel
{
	[FromRoute] public string Domain { get; set; } = default!;

	public void OnGet()
	{

	}
}