using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Blog;

public class Page([FromServices] LoaderFactory loader) : PageModel
{
	[FromRoute]
	public string Slug { get; set; }

	[FromRoute]
	public int Year { get; set; }

	[FromRoute]
	public int Month { get; set; }

	[FromRoute]
	public int Day { get; set; }

	public LoadDate Blog { get; set; } = loader.LoadFrom<LoadDate, MarkdownDoc>("_blog");

	public LoadDirectory Authors { get; set; } = loader.LoadFrom<LoadDirectory, MarkdownAuthor>("_authors");

	public MarkdownDoc? Source =>  Blog.GetByDate<MarkdownDoc>(new DateTime(Year, Month, Day), Slug);

	public Dictionary<string, MarkdownAuthor> SourceAuthors => Source != null
		? Authors.GetAll<MarkdownAuthor>().ToDictionary(a => a.Id)
		: new Dictionary<string, MarkdownAuthor>(0);

	public HtmlString Html => new(Source?.Html);

	public void OnGet()
	{
		Blog.Reload(Source!);
	}
}