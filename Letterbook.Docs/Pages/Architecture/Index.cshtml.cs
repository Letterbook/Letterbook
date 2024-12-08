using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Architecture;

public class Index([FromServices] MarkdownLoaderFactory<LoadDirectory> factory) : PageModel
{
	public LoadDirectory<MarkdownAdr> Docs { get; set; } = (LoadDirectory<MarkdownAdr>)factory.LoadFrom<MarkdownAdr>("_adr");
}