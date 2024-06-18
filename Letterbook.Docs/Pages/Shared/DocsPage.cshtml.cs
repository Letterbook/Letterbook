using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Docs.Pages.Shared;

public class DocsPage : PageModel
{
    public string? Brand { get; set; }
    public string Slug { get; set; }
    public string Folder { get; set; }
    public MarkdownMenu? DefaultMenu { get; set; }
    public MarkdownFileInfo? Doc { get; set; }
    public Func<dynamic?, object>? Header { get; set; }
    public Func<dynamic?, object>? Footer { get; set; }
    public string TitleClass { get; set; } = "text-4xl tracking-tight font-extrabold text-gray-900 dark:text-gray-50 sm:text-5xl md:text-6xl";
    public bool HideTitle { get; set; }
    public bool HideNavigation { get; set; }
    public bool HideDocumentMap { get; set; }
    public bool HideEditOnGitHub { get; set; }
    public Action<List<MarkdownMenu>>? SidebarFilter { get; set; }

    public DocsPage Init(Microsoft.AspNetCore.Mvc.RazorPages.Page page, MarkdownPages markdown)
    {
        Doc = markdown.GetBySlug($"{Folder}/{Slug}");
        if (Doc == null)
        {
            page.Response.Redirect("/404");
            return this;
        }

        if (!string.IsNullOrEmpty(Brand))
        {
            page.ViewContext.ViewData["Brand"] = Brand;
        }
        
        page.ViewContext.ViewData["Title"] = Doc.Title;
        return this;
    }
}