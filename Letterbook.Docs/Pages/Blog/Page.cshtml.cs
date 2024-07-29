using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Blog;

public class Page([FromKeyedServices("_blog")] MarkdownChrono blog) : PageModel
{
	private readonly IServiceProvider _services;
	private MarkdownFileBase? _source;

	[FromRoute]
	public string Slug { get; set; }

	[FromRoute]
	public int Year { get; set; }

	[FromRoute]
	public int Month { get; set; }

	[FromRoute]
	public int Day { get; set; }

	public MarkdownChrono Blog { get; set; } = blog;

	public MarkdownFileBase? Source => _source ??= Blog.GetByDate(new DateTime(Year, Month, Day), Slug);

	public HtmlString Doc => new HtmlString(Source?.Preview);
}