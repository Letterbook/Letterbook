using Letterbook.Docs.Markdown;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Blog;

public class Index([FromServices] MarkdownFilesFactory<Markdown.MarkdownDate> blog) : PageModel
{
	public Markdown.MarkdownDate Blog { get; set; } = blog.GetMarkdown("_blog");

	public List<Index> GetStaticProps(RenderContext ctx)
	{
		throw new NotImplementedException();
	}
}