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
	private readonly IAccountService _accounts;
	private readonly IModerationEventPublisher _moderationEvents;
	private readonly IActivityScheduler _activity;

	public ModerationService(IDataAdapter data, IAuthorizationService authz, IProfileEventPublisher profileEvents, IAccountService accounts, IModerationEventPublisher moderationEvents, IActivityScheduler activity)
	{
		_data = data;
		_authz = authz;
		_profileEvents = profileEvents;
		_accounts = accounts;
		_moderationEvents = moderationEvents;
		_activity = activity;
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
		var authz = _authz.Create<ModerationReport>(_claims);
		if (!authz.Allowed)
			throw CoreException.Unauthorized(authz);
		if (report.Subjects.Count == 0)
			throw CoreException.InvalidRequest("Report must include a subject profile");

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
		await _moderationEvents.Created(report, reporterId, _claims);

		foreach (var subject in report.Subjects)
		{
			await _profileEvents.Reported(subject, reporter);
		}

		foreach (var inbox in report.Forwarded)
		{
			await _activity.Report(inbox, report);
		}

		return report;
	}

	public async Task<ModerationReport> AddRemark(ModerationReportId id, ModerationRemark remark)
	{
		if (await _data.ModerationReports(id).FirstOrDefaultAsync() is not { } report)
			throw CoreException.MissingData<ModerationReport>(id);
		if (await _data.Accounts(remark.Author.Id).FirstOrDefaultAsync() is not {} moderator)
			throw CoreException.MissingData<Account>(remark.Author.Id);

		var authz = _authz.Create(_claims, remark);
		if (!authz.Allowed)
			throw CoreException.Unauthorized(authz);

		remark.Author = moderator;
		remark.Report = report;
		report.Remarks.Add(remark);
		report.Updated = DateTimeOffset.UtcNow;
		await _data.Commit();

		return report;
	}

	public async Task<ModerationReport> AssignModerator(ModerationReportId reportId, Guid moderatorAccountId, bool remove = false)
	{
		if (await _data.ModerationReports(reportId).FirstOrDefaultAsync() is not {} report)
			throw CoreException.MissingData<ModerationReport>(reportId);
		if (await _data.LookupAccount(moderatorAccountId) is not {} moderator)
			throw CoreException.MissingData<Account>(moderatorAccountId);

		var canAssign = _authz.Update(_claims, report);
		if(!canAssign)
			throw CoreException.Unauthorized(canAssign);

		if (remove)
		{
			report.Moderators.Remove(moderator);
			await _data.Commit();
			return report;
		}

		var isModerator = _authz.Update(await _accounts.LookupClaims(moderator), report);
		if(!isModerator)
			throw CoreException.Unauthorized(isModerator);
		report.Moderators.Add(moderator);
		report.Updated = DateTimeOffset.UtcNow;

		await _data.Commit();
		await _moderationEvents.Assigned(report, moderatorAccountId, _claims);
		return report;
	}

	public async Task<ModerationReport> UpdateReport(ModerationReportId reportId, ModerationReport updated, Guid accountId,
		ModerationReport.Scope sendScope, bool resend = false)
	{
		if (await _data.ModerationReports(reportId)
			    .Include(r => r.Policies)
			    .Include(r => r.Moderators)
			    .FirstOrDefaultAsync() is not {} report)
			throw CoreException.MissingData<ModerationReport>(reportId);

		var mods = await _data.Accounts(updated.Moderators.Select(a => a.Id).ToArray())
			.ToDictionaryAsync(a => a.Id);
		var policies = await _data.Policies(updated.Policies.Select(p => p.Id).ToArray())
			.ToDictionaryAsync(p => p.Id);

		var newModerators = updated.Moderators.Select(m => m.Id).Except(report.Moderators.Select(m => m.Id));
		report.Moderators = updated.Moderators.Converge(mods, account => account.Id).ToHashSet();
		report.Policies = updated.Policies.Converge(policies, policy => policy.Id).ToHashSet();

		var closed = updated.Closed < DateTimeOffset.UtcNow && report.Closed > DateTimeOffset.UtcNow;
		var reopened = report.Closed > DateTimeOffset.UtcNow && updated.Closed <= DateTimeOffset.UtcNow;
		report.Closed = updated.Closed;

		var forwardTo = updated.Forwarded.Where(inbox => report.ForwardTo(inbox, resend)).ToList();

		report.Updated = DateTimeOffset.UtcNow;


		await _data.Commit();
		foreach (var moderator in newModerators)
		{
			await _moderationEvents.Assigned(report, moderator, _claims);
		}
		if (closed) await _moderationEvents.Closed(report, accountId, _claims);
		if (reopened) await _moderationEvents.Reopened(report, accountId, _claims);
		if (report.IsClosed()) return report;

		foreach (var inbox in forwardTo)
		{
			await _activity.Report(inbox, report, sendScope);
		}

		return report;
	}

	public async Task<ModerationReport> CloseReport(ModerationReportId reportId, Guid moderatorId, bool reopen = false)
	{
		if (await _data.ModerationReports(reportId).FirstOrDefaultAsync() is not {} report)
			throw CoreException.MissingData<ModerationReport>(reportId);

		var allowed = _authz.Update(_claims, report);
		if(!allowed)
			throw CoreException.Unauthorized(allowed);

		var closed = false;
		if (reopen) report.Closed = DateTimeOffset.MaxValue;
		else closed = report.Close();


		await _data.Commit();
		if (closed) await _moderationEvents.Closed(report, moderatorId, _claims);
		if (reopen) await _moderationEvents.Reopened(report, moderatorId, _claims);
		return report;
	}

	public IAsyncEnumerable<ModerationReport> Search(string query, bool assignedToMe = true, bool unassigned = true, bool includeClosed = false)
	{
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<ModerationReport> FindCreatedBy(ProfileId reporter, bool includeClosed = false)
	{
		return _data.Profiles(reporter)
			.Include(p => p.Reports)
			.SelectMany(p => p.Reports)
			.OrderByDescending(r => r.Created)
			.Where(r => includeClosed || r.Closed > DateTimeOffset.UtcNow)
			.AsAsyncEnumerable()
			.TakeWhile(report => _authz.View(_claims, report));
	}

	public IAsyncEnumerable<ModerationReport> FindRelatedTo(PostId post, bool includeClosed = false)
	{
		return _data.Posts(post)
			.Include(p => p.RelatedReports)
			.SelectMany(p => p.RelatedReports)
			.OrderByDescending(r => r.Created)
			.Where(r => includeClosed || r.Closed > DateTimeOffset.UtcNow)
			.AsAsyncEnumerable()
			.TakeWhile(report => _authz.View(_claims, report));
	}

	public IAsyncEnumerable<ModerationReport> FindAssigned(Guid moderator, bool includeClosed = false)
	{
		return _data.Accounts(moderator)
			.Include(a => a.ModeratedReports)
			.SelectMany(a => a.ModeratedReports)
			.OrderByDescending(r => r.Created)
			.Where(r => includeClosed || r.Closed > DateTimeOffset.UtcNow)
			.AsAsyncEnumerable()
			.TakeWhile(report => _authz.View(_claims, report));
	}

	public IAsyncEnumerable<ModerationReport> FindRelatedTo(ProfileId subject, bool includeClosed = false)
	{
		return _data.Profiles(subject)
			.Include(p => p.Reports)
			.SelectMany(p => p.Reports)
			.OrderByDescending(r => r.Created)
			.Where(r => includeClosed || r.Closed > DateTimeOffset.UtcNow)
			.AsAsyncEnumerable()
			.TakeWhile(report => _authz.View(_claims, report));
	}

	public IAsyncEnumerable<ModerationPolicy> ListPolicies(bool includeRetired = false)
	{
		return _data.AllPolicies()
			.Where(p => includeRetired || p.Retired >= DateTimeOffset.UtcNow)
			.AsAsyncEnumerable();
	}

	public async Task<ModerationPolicy> AddPolicy(ModerationPolicy policy)
	{
		var allowed = _authz.Create<ModerationPolicy>(_claims);
		if (!allowed)
			throw CoreException.Unauthorized(allowed);

		_data.Add(policy);
		await _data.Commit();
		return policy;
	}

	public async Task<ModerationPolicy> RetirePolicy(ModerationPolicyId policyId, bool restore = false)
	{
		var allowed = _authz.Update<ModerationPolicy>(_claims);
		if (!allowed)
			throw CoreException.Unauthorized(allowed);

		if (await _data.Policies(policyId).FirstOrDefaultAsync() is not { } policy)
			throw CoreException.MissingData<ModerationPolicy>(policyId);

		if (restore)
		{
			policy.Retired = DateTimeOffset.MaxValue;
			await _data.Commit();

			return policy;
		}

		if (policy.Retire())
			await _data.Commit();

		return policy;
	}

	public IAuthzModerationService As(IEnumerable<Claim> claims)
	{
		_claims = claims;
		return this;
	}
}