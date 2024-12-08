using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Docs;

public class Index([FromServices] LoaderFactory<LoadCategories> categories) : PageModel
{
	public LoadCategories Categories { get; set; } = categories.GetMarkdown("_pages");
}