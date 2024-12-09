using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Docs;

public class Page([FromServices] LoaderFactory factory) : PageModel
{
	[FromRoute]
	public string Slug { get; set; }

	public LoadDirectory Docs { get; set; } = factory.LoadFrom<LoadDirectory, MarkdownAdr>("_adr");

	public MarkdownAdr? Doc => Docs.GetBySlug<MarkdownAdr>(Slug);
}