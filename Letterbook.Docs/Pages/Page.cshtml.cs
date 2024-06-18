using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs;

public class Page : PageModel
{
    [FromRoute]
    public string Slug { get; set; }
}