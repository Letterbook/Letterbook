using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Docs;

public class Index([FromServices] MarkdownFilesFactory<MarkdownCategories> categories) : PageModel
{
	public MarkdownCategories Categories { get; set; } = categories.GetMarkdown("_pages");
}