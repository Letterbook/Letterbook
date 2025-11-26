using AutoMapper.QueryableExtensions;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Web.Projections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Web.Areas.Administration.Pages;

public class Report : PageModel
{
	private readonly IModerationService _moderation;
	private readonly IProfileService _profiles;
	private readonly IAuthorizationService _authz;

	public Models.ModerationReport Self { get; set; } = default!;
	public Dictionary<Models.ProfileId, ReportProfile> Profiles { get; set; } = default!;
	public Guid AccountId { get; set; }
	public List<Models.ProfileId> RelatedProfileIDs { get; set; } = [];
	public List<Models.ProfileId> SubjectIDs { get; set; } = [];

	[FromRoute] public Models.ModerationReportId ReportId { get; set; }
	[FromForm] public string ModeratorRemark { get; set; } = default!;

	public Report(IModerationService moderation, IProfileService profiles, IAuthorizationService authz)
	{
		_moderation = moderation;
		_profiles = profiles;
		_authz = authz;
	}

	public async Task<IActionResult> OnGet()
	{
		if (User.Identity == null || !User.Identity.IsAuthenticated)
			return Challenge();
		if (!User.Claims.TryGetAccountId(out var accountId))
			return Challenge();
		if (!_authz.Update<Models.ModerationReport>(User.Claims))
			return Forbid();
		if (await _moderation.As(User.Claims).LookupReport(ReportId) is not { } report)
			return NotFound();
		Self = report;
		AccountId = accountId;

		await InitPage();

		//temporary
		if (Self.Reporter is not null)
			Self.Forwarded.Add(new Uri("https://social.neighbor.example"));

		return Page();
	}

	private async Task InitPage()
	{
		SubjectIDs = Self.Subjects.Select(p => p.Id).ToList();
		RelatedProfileIDs = Self.RelatedPosts
			.SelectMany(p => p.Creators)
			.Append(Self.Reporter)
			.WhereNotNull()
			.Select(p => p.Id)
			.Except(SubjectIDs)
			.Distinct()
			.ToList();

		Profiles = await _profiles.As(User.Claims)
			.QueryProfiles([..SubjectIDs, ..RelatedProfileIDs])
			.TagWithCallSite()
			.ProjectTo<Projections.ReportProfile>(Projections.ReportProfile.FromCoreModel(ReportId))
			.ToDictionaryAsync(p => p.Id);
	}

	public async Task<IActionResult> OnPostRemark()
	{
		if (User.Identity == null || !User.Identity.IsAuthenticated)
			return Challenge();
		if (!User.Claims.TryGetAccountId(out var accountId))
			return Challenge();
		if (!_authz.Update<Models.ModerationReport>(User.Claims))
			return Forbid();
		if (await _moderation.As(User.Claims).LookupReport(ReportId) is not { } report)
			return NotFound();

		var remark = new Models.ModerationRemark
		{
			Report = report,
			Author = new Models.Account(){Id = accountId},
			Created = DateTimeOffset.UtcNow,
			Text = ModeratorRemark
		};
		AccountId = accountId;
		Self = await _moderation.As(User.Claims).AddRemark(ReportId, remark);

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostClose()
	{
		if (User.Identity == null || !User.Identity.IsAuthenticated)
			return Challenge();
		if (!User.Claims.TryGetAccountId(out var accountId))
			return Challenge();
		if (!_authz.Update<Models.ModerationReport>(User.Claims))
			return Forbid();

		await _moderation.As(User.Claims).CloseReport(ReportId, accountId);

		return RedirectToPage();
	}
}