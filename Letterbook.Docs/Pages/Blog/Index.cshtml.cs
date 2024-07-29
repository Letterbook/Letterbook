using Microsoft.AspNetCore.Components;

namespace Letterbook.Docs.Pages.Blog;

public class Index([FromKeyedServices("_blog")] MarkdownChrono blog) : Letterbook.Docs.Page, IRenderStatic<Index>
{
	public MarkdownChrono Blog { get; set; } = blog;

	public List<Index> GetStaticProps(RenderContext ctx)
	{
		throw new NotImplementedException();
	}
}