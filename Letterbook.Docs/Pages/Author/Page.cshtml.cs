using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Author;

public class Page([FromServices] LoaderFactory loader) : PageModel
{
	public LoadDate Blog { get; set; } = loader.LoadFrom<LoadDate, MarkdownDoc>("_blog");
	public LoadDirectory Authors { get; set; } = loader.LoadFrom<LoadDirectory, MarkdownAuthor>("_authors");

	[FromRoute]
	public string Slug { get; set; }

	public MarkdownAuthor? Doc => Authors.GetBySlug<MarkdownAuthor>(Slug);

	public IEnumerable<MarkdownDoc> Articles => Doc != null ? Blog.GetAll<MarkdownDoc>().Where(d => d.Authors.Contains(Doc.Id)) : [];
}