using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Architecture;

public class Index([FromServices] LoaderFactory factory) : PageModel
{
	public LoadDirectory Docs { get; set; } = factory.LoadFrom<LoadDirectory, MarkdownAdr>("_adr");
}