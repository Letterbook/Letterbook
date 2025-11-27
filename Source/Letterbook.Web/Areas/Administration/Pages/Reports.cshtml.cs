using Letterbook.Core;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class Reports : PageModel
{
	private readonly IModerationService _moderation;
	private readonly IAuthorizationService _authz;

	public List<Models.ModerationReport> List { get; set; } = [];

	public Reports(IModerationService moderation, IAuthorizationService authz)
	{
		_moderation = moderation;
		_authz = authz;
	}

	public async Task<IActionResult> OnGet()
	{
		if (User.Identity == null || !User.Identity.IsAuthenticated)
			return Challenge();
		if (!User.Claims.TryGetAccountId(out var id))
			return Challenge();
		if (!_authz.List<Reports>(User.Claims))
			return Forbid();

		var reports = _moderation.As(User.Claims).ListReports();
		await foreach (var report in reports)
		{
			List.Add(report);
		}

		return Page();
	}
}