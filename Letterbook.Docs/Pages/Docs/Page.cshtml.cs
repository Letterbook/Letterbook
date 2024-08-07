using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Docs;

public class Page([FromServices] MarkdownFilesFactory<MarkdownCategories> categories) : PageModel
{
	[FromRoute]
	public required string Category { get; set; }

	[FromRoute]
	public required string Slug { get; set; }

	public MarkdownCategories Categories { get; set; } = categories.GetMarkdown("_pages");
	public MarkdownCategory? Source => Categories.GetByCategory(Category == "page" ? null : Category, Slug);
	public HtmlString Html => new(Source?.Html);

}