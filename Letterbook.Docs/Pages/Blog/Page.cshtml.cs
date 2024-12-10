using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Blog;

public class Page([FromServices] LoaderFactory blog) : PageModel
{
	[FromRoute]
	public string Slug { get; set; }

	[FromRoute]
	public int Year { get; set; }

	[FromRoute]
	public int Month { get; set; }

	[FromRoute]
	public int Day { get; set; }

	public LoadDate Blog { get; set; } = blog.LoadFrom<LoadDate, MarkdownDoc>("_blog");

	public MarkdownDoc? Source =>  Blog.GetByDate<MarkdownDoc>(new DateTime(Year, Month, Day), Slug);

	public HtmlString Html => new(Source?.Html);
}