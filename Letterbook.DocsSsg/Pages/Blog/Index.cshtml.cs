using Letterbook.DocsSsg.Markdown;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.DocsSsg.Pages.Blog;

public class Index([FromKeyedServices("_blog")]MarkdownChrono blog) : PageModel
{
	public MarkdownChrono Blog { get; } = blog;
	public List<MarkdownDoc> Posts => Blog.Files;
}