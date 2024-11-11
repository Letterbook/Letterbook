using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages;

public class Index([FromServices] MarkdownFilesFactory<MarkdownCategories> categories) : PageModel
{
	public MarkdownCategories Pages { get; set; } = categories.GetMarkdown("_pages");

	public MarkdownCategory? Source => Pages.GetByCategory(null, "index");

	public HtmlString Html => new(Source?.Html);
}