using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Blog;

public class Index([FromServices] LoaderFactory blog) : PageModel
{
	public Markdown.LoadDate Blog { get; set; } = blog.LoadFrom<LoadDate, MarkdownDoc>("_blog");

	public List<Index> GetStaticProps(RenderContext ctx)
	{
		throw new NotImplementedException();
	}
}