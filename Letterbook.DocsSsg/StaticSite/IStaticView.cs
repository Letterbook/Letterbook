using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.DocsSsg.StaticSite;

public interface IStaticView<T> where T : PageModel
{
	public IEnumerable<T> GetStaticPages();
}