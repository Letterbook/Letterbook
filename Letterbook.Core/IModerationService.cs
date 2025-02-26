using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IAuthzModerationService
{
	/// <summary>
	/// Lookup a single report by ID
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public Task<ModerationReport?> LookupReport(ModerationReportId id);

	/// <summary>
	/// Open a new report
	/// </summary>
	/// <param name="reporterId"></param>
	/// <param name="report"></param>
	/// <returns></returns>
	public Task<ModerationReport> CreateReport(ProfileId reporterId, ModerationReport report);

	/// <summary>
	/// Add a moderator comment to a report
	/// </summary>
	/// <param name="id"></param>
	/// <param name="remark"></param>
	/// <returns></returns>
	public Task<ModerationReport> AddRemark(ModerationReportId id, ModerationRemark remark);

	/// <summary>
	/// Add or remove an assigned moderator to a report
	/// </summary>
	/// <param name="reportId"></param>
	/// <param name="moderatorAccountId"></param>
	/// <param name="remove"></param>
	/// <returns></returns>
	public Task<ModerationReport> AssignModerator(ModerationReportId reportId, Guid moderatorAccountId, bool remove = false);

	/// <summary>
	/// Make certain kinds of updates to a report. Not all updates are permitted.
	/// </summary>
	/// <remarks>
	/// It is not permitted to change the report summary, the related posts and subjects, or the remarks.
	/// Those changes would make this a fundamentally different report, or obscure its history.
	/// Instead of making those major changes, consider closing this report and opening a new one.
	/// </remarks>
	/// <param name="reportId"></param>
	/// <param name="updated"></param>
	/// <param name="accountId"></param>
	/// <param name="sendScope"></param>
	/// <param name="resend"></param>
	/// <returns></returns>
	public Task<ModerationReport> UpdateReport(ModerationReportId reportId, ModerationReport updated, Guid accountId,
		ModerationReport.Scope sendScope = ModerationReport.Scope.Profile, bool resend = false);

	/// <summary>
	/// Close or reopen a report
	/// </summary>
	/// <param name="reportId"></param>
	/// <param name="moderatorId"></param>
	/// <param name="reopen"></param>
	/// <returns></returns>
	public Task<ModerationReport> CloseReport(ModerationReportId reportId, Guid moderatorId, bool reopen = false);

	/// <summary>
	/// Search for reports
	/// </summary>
	/// <param name="query"></param>
	/// <param name="assignedToMe"></param>
	/// <param name="unassigned"></param>
	/// <param name="includeClosed"></param>
	/// <returns></returns>
	public IAsyncEnumerable<ModerationReport> Search(string query, bool assignedToMe = true, bool unassigned = true, bool includeClosed = false);

	/// <summary>
	/// Find reports created by this profile
	/// </summary>
	/// <param name="reporter"></param>
	/// <returns></returns>
	public IAsyncEnumerable<ModerationReport> FindCreatedBy(ProfileId reporter, bool includeClosed = false);

	/// <summary>
	/// Find reports related to this post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public IAsyncEnumerable<ModerationReport> FindRelatedTo(PostId post, bool includeClosed = false);

	/// <summary>
	/// Find reports assigned to this moderator
	/// </summary>
	/// <param name="moderator"></param>
	/// <returns></returns>
	public IAsyncEnumerable<ModerationReport> FindAssigned(Guid moderator, bool includeClosed = false);

	/// <summary>
	/// Find reports related to this subject
	/// </summary>
	/// <param name="subject"></param>
	/// <returns></returns>
	public IAsyncEnumerable<ModerationReport> FindRelatedTo(ProfileId subject, bool includeClosed = false);

	public IAsyncEnumerable<ModerationPolicy> ListPolicies(bool includeRetired = false);
	public Task<ModerationPolicy> AddPolicy(ModerationPolicy policy);
	public Task<ModerationPolicy> RetirePolicy(ModerationPolicyId policyId, bool restore = false);
}

public interface IModerationService
{
	public IAuthzModerationService As(IEnumerable<Claim> claims);
}