using Letterbook.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Areas.Administration.Pages;

public class Report : PageModel
{
	private readonly IModerationService _moderation;
	private readonly IAuthorizationService _authz;
	public Models.ModerationReport Self { get; set; } = default!;

	[FromRoute] public Models.ModerationReportId ReportId { get; set; }

	public Report(IModerationService moderation, IAuthorizationService authz)
	{
		_moderation = moderation;
		_authz = authz;
	}

	public async Task<IActionResult> OnGet()
	{
		if (!_authz.Update<Models.ModerationReport>(User.Claims))
			return Forbid();
		if (await _moderation.As(User.Claims).LookupReport(ReportId) is not { } report)
			return NotFound();
		Self = report;

		//temporary
		if (Self.Reporter is not null)
			Self.Forwarded.Add(new Uri("https://social.neighbor.example"));

		return Page();
	}
}