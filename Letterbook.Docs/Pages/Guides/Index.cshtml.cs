using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Guides;

public class Index([FromServices] LoaderFactory categories) : PageModel
{
	public LoadCategories Categories { get; set; } = categories.LoadFrom<LoadCategories, MarkdownCategory>("_pages");
}