using System.Security.Claims;
using Letterbook.Core.Adapters;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Core;

public class ModerationService : IModerationService, IAuthzModerationService
{
	private IEnumerable<Claim> _claims = [];
	private readonly IDataAdapter _data;
	private readonly IAuthorizationService _authz;
	private readonly IProfileEventPublisher _profileEvents;

	public ModerationService(IDataAdapter data, IAuthorizationService authz, IProfileEventPublisher profileEvents)
	{
		_data = data;
		_authz = authz;
		_profileEvents = profileEvents;
	}

	public async Task<ModerationReport?> LookupReport(ModerationReportId id)
	{
		if (await _data.ModerationReports(id).FirstOrDefaultAsync() is not {} report)
			throw CoreException.MissingData<ModerationReport>(id);

		var authz = _authz.View(_claims, report);
		if (!authz.Allowed)
			throw CoreException.Unauthorized(authz);

		return report;
	}

	public async Task<ModerationReport> CreateReport(ProfileId reporterId, ModerationReport report)
	{
		var profiles = await _data.Profiles([report.Reporter!.Id, ..report.Subjects.Select(p => p.Id)])
			.Distinct()
			.ToDictionaryAsync(profile => profile.Id);
		var posts = await _data.Posts(report.RelatedPosts.Select(p => p.Id).ToArray())
			.Distinct()
			.ToDictionaryAsync(post => post.Id);

		if (!profiles.TryGetValue(reporterId, out var reporter))
			throw CoreException.MissingData<ProfileId>($"Reporter {reporterId} not found");

		var subjects = report.Subjects.Converge(profiles, p => p.Id).ToHashSet();
		if (subjects.Count < report.Subjects.Count)
			throw CoreException.MissingData<ProfileId>($"Subject(s) {report.Subjects.Except(subjects).Select(p => p.Id)} not found");

		var relatedPosts = report.RelatedPosts.Converge(posts, p => p.Id).ToHashSet();
		if (relatedPosts.Count < report.RelatedPosts.Count)
			throw CoreException.MissingData<ProfileId>($"Post(s) {report.RelatedPosts.Except(relatedPosts).Select(p => p.Id)} not found");

		report.Reporter = reporter;
		report.Subjects = subjects;
		report.RelatedPosts = relatedPosts;

		_data.Add(report);
		await _data.Commit();

		foreach (var subject in report.Subjects)
		{
			await _profileEvents.Reported(subject, reporter);
		}

		return report;
	}

	public async Task<ModerationReport> AddRemark(ModerationReportId id, ModerationRemark remark)
	{
		throw new NotImplementedException();
	}
	public async Task<ModerationReport> AssignModerator(ModerationReportId reportId, Guid moderatorAccountId, bool remove = false)
	{
		throw new NotImplementedException();
	}
	public async Task<ModerationReport> CloseReport(ModerationReportId reportId, bool reopen = false)
	{
		throw new NotImplementedException();
	}
	public IAsyncEnumerable<ModerationReport> Search(string query, bool assignedToMe = true, bool unassigned = true, bool includeClosed = false)
	{
		throw new NotImplementedException();
	}
	public IAsyncEnumerable<ModerationReport> FindRelated(PostId post)
	{
		throw new NotImplementedException();
	}
	public IAsyncEnumerable<ModerationReport> FindRelated(ProfileId post)
	{
		throw new NotImplementedException();
	}

	public IAuthzModerationService As(IEnumerable<Claim> claims)
	{
		_claims = claims;
		return this;
	}
}