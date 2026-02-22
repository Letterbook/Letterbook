using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

public class Search : PageModel
{
	private readonly ISearchService _svc;

	public IList<Models.Profile> Profiles { get; set; } = [];
	public IList<Models.Post> Posts { get; set; } = [];
	[FromQuery]
	public string? SearchType { get; set; } = null;
	[FromQuery]
	public string? Query { get; set; } = null;

	public Search(ISearchService svc)
	{
		_svc = svc;
	}

	public async Task<IActionResult> OnGet()
	{
		if (User.Identity?.IsAuthenticated != true)
			return Challenge();
		if (Query is null) return Page();
		var svc = _svc.As(User.Claims);

		switch (SearchType)
		{
			case "profiles":
				await foreach (var r in svc.SearchProfiles(Query, HttpContext.RequestAborted))
				{
					Profiles.Add(r);
				}
				break;
			case "posts":
				await foreach (var r in svc.SearchPosts(Query, HttpContext.RequestAborted))
				{
					Posts.Add(r);
				}
				break;
			default:
				await foreach (var r in svc.SearchAll(Query, HttpContext.RequestAborted))
				{
					switch (r)
					{
						case Models.Post post:
							Posts.Add(post);
							break;
						case Models.Profile profile:
							Profiles.Add(profile);
							break;
					}
				}
				break;
		}

		return Page();
	}
}