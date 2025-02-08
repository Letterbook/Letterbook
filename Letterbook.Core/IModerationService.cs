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
	/// Close or reopen a report
	/// </summary>
	/// <param name="reportId"></param>
	/// <param name="reopen"></param>
	/// <returns></returns>
	public Task<ModerationReport> CloseReport(ModerationReportId reportId, bool reopen = false);

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
	public IAsyncEnumerable<ModerationReport> FindCreatedBy(ProfileId reporter);

	/// <summary>
	/// Find reports related to this post
	/// </summary>
	/// <param name="post"></param>
	/// <returns></returns>
	public IAsyncEnumerable<ModerationReport> FindRelatedTo(PostId post);

	/// <summary>
	/// Find reports related to this subject
	/// </summary>
	/// <param name="subject"></param>
	/// <returns></returns>
	public IAsyncEnumerable<ModerationReport> FindRelatedTo(ProfileId subject);
}

public interface IModerationService
{
	public IAuthzModerationService As(IEnumerable<Claim> claims);
}