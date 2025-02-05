using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IAuthzModerationService
{
	public Task<ModerationReport?> LookupReport(ModerationReportId id);
	public Task<ModerationReport> CreateReport(ProfileId reporterId, ModerationReport report);
	public Task<ModerationReport> AddRemark(ModerationReportId id, ModerationRemark remark);
	public Task<ModerationReport> AssignModerator(ModerationReportId reportId, Guid moderatorAccountId, bool remove = false);
	public Task<ModerationReport> CloseReport(ModerationReportId reportId, bool reopen = false);
	public IAsyncEnumerable<ModerationReport> Search(string query, bool assignedToMe = true, bool unassigned = true, bool includeClosed = false);
	public IAsyncEnumerable<ModerationReport> FindRelated(PostId post);
	public IAsyncEnumerable<ModerationReport> FindRelated(ProfileId post);
}

public interface IModerationService
{
	public IAuthzModerationService As(IEnumerable<Claim> claims);
}