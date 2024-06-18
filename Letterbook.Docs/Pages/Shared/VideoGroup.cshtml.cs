// BSD License: https://docs.servicestack.net/BSD-LICENSE.txt
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Shared;

public class VideoGroup : PageModel
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Group { get; set; }
    public string Background { get; set; } = "bg-white dark:bg-black";
    public string Border { get; set; } = "border border-slate-300 dark:border-slate-600";
    public string? LearnMore { get; set; }
}