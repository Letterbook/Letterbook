using System.Security.Claims;
using Bogus;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;

namespace Letterbook.Web.Mocks;

public class MockModerationService : IModerationService, IAuthzModerationService
{
	private ModerationService _moderationService;
	private static List<Models.ModerationReport> _mockReports = new FakeReport().Generate(55).ToList();
	private static List<Models.Account> _mockAccounts = new FakeAccount().Generate(3).ToList();

	public MockModerationService(ModerationService moderationService)
	{
		_moderationService = moderationService;
	}


	public async Task<Models.ModerationReport?> LookupReport(Models.ModerationReportId id)
	{
		if (_mockReports.FirstOrDefault(r => r.Id == id) is { } report)
			return report;
		return await _moderationService.LookupReport(id);
	}
	public Task<Models.ModerationReport> CreateReport(Models.ProfileId reporterId, Models.ModerationReport report)
	{
		return _moderationService.CreateReport(reporterId, report);
	}
	public Task<Models.ModerationReport> AddRemark(Models.ModerationReportId id, Models.ModerationRemark remark)
	{
		return _moderationService.AddRemark(id, remark);
	}
	public Task<Models.ModerationReport> AssignModerator(Models.ModerationReportId reportId, Guid moderatorAccountId, bool remove = false)
	{
		return _moderationService.AssignModerator(reportId, moderatorAccountId, remove);
	}

	public Task<Models.ModerationReport> UpdateReport(Models.ModerationReportId reportId, Models.ModerationReport updated, Guid accountId, Models.ModerationReport.Scope sendScope = Models.ModerationReport.Scope.Profile,
		bool resend = false)
	{
		return _moderationService.UpdateReport(reportId, updated, accountId, sendScope, resend);
	}

	public Task<Models.ModerationReport> CloseReport(Models.ModerationReportId reportId, Guid moderatorId, bool reopen = false)
	{
		return _moderationService.CloseReport(reportId, moderatorId, reopen);
	}
	public Task<Models.ModerationReport> ReceiveReport(Uri reporterId, Models.ModerationReport report)
	{
		return _moderationService.ReceiveReport(reporterId, report);
	}

	/// <summary>
	/// Mocked reports
	/// </summary>
	/// <param name="cursor"></param>
	/// <param name="includeClosed"></param>
	/// <param name="oldestFirst"></param>
	/// <param name="limit"></param>
	/// <returns></returns>
	public IAsyncEnumerable<Models.ModerationReport> ListReports(DateTimeOffset? cursor = null, bool includeClosed = false, bool oldestFirst = false, int limit = 20)
	{
		var list = oldestFirst ? _mockReports.OrderBy(r => r.Created) : _mockReports.OrderByDescending(r => r.Created);
		var faker = new Faker();

		return list.Take(limit).Select(e =>
		{
			if (faker.Random.Bool(0.15f) && e.Moderators.Count == 0)
			{
				e.Moderators.Add(faker.PickRandom(_mockAccounts));
			}

			if (e.RelatedPosts.Count == 0)
			{
				var fakePost = new FakePost(e.Subjects, 1, null);
				var posts = fakePost.UseSeed(e.GetHashCode()).Generate(faker.Random.Int(0, 3));
				e.RelatedPosts = posts;
			}
			return e;
		}).ToAsyncEnumerable();
	}

	public IAsyncEnumerable<Models.ModerationReport> SearchReports(string query, bool assignedToMe = true, bool unassigned = true, bool includeClosed = false)
	{
		return _moderationService.SearchReports(query, assignedToMe, unassigned, includeClosed);
	}

	public IQueryable<Models.ModerationReport> QueryReport(Models.ModerationReportId id)
	{
		return _moderationService.QueryReport(id);
	}

	public IAsyncEnumerable<Models.ModerationReport> FindCreatedBy(Models.ProfileId reporter, bool includeClosed = false)
	{
		return _moderationService.FindCreatedBy(reporter, includeClosed);
	}
	public IAsyncEnumerable<Models.ModerationReport> FindRelatedTo(Models.PostId post, bool includeClosed = false)
	{
		return _moderationService.FindRelatedTo(post, includeClosed);
	}
	public IAsyncEnumerable<Models.ModerationReport> FindAssigned(Guid moderator, bool includeClosed = false)
	{
		return _moderationService.FindAssigned(moderator, includeClosed);
	}
	public IAsyncEnumerable<Models.ModerationReport> FindRelatedTo(Models.ProfileId subject, bool includeClosed = false)
	{
		return _moderationService.FindRelatedTo(subject, includeClosed);
	}
	public IAsyncEnumerable<Models.ModerationPolicy> ListPolicies(bool includeRetired = false)
	{
		return _moderationService.ListPolicies(includeRetired);
	}
	public Task<Models.ModerationPolicy> AddPolicy(Models.ModerationPolicy policy)
	{
		return _moderationService.AddPolicy(policy);
	}
	public Task<Models.ModerationPolicy> RetirePolicy(Models.ModerationPolicyId policyId, bool restore = false)
	{
		return _moderationService.RetirePolicy(policyId, restore);
	}
	public Task<ICollection<Models.Restrictions>> GetOrInitPeerRestrictions(Uri peerId)
	{
		return _moderationService.GetOrInitPeerRestrictions(peerId);
	}
	public Task<Models.Peer> GetOrInitPeer(Uri peerId)
	{
		return _moderationService.GetOrInitPeer(peerId);
	}
	public Task<Models.Peer> SetPeerRestriction(Uri peerId, Models.Restrictions restriction, DateTimeOffset expiration)
	{
		return _moderationService.SetPeerRestriction(peerId, restriction, expiration);
	}
	public Task<ICollection<Models.Peer>> ImportPeerRestrictions(ICollection<Models.Peer> imports, ModerationService.MergeStrategy strategy)
	{
		return _moderationService.ImportPeerRestrictions(imports, strategy);
	}
	public Task<Models.Peer> RemovePeerRestriction(Uri peerId, Models.Restrictions restriction)
	{
		return _moderationService.RemovePeerRestriction(peerId, restriction);
	}
	public IAsyncEnumerable<Models.Peer> ListPeers(Uri? peerIdCursor = null, int limit = 20)
	{
		return _moderationService.ListPeers(peerIdCursor, limit);
	}
	public Task<Models.Peer?> LookupPeer(Uri peerId)
	{
		return _moderationService.LookupPeer(peerId);
	}
	public IAsyncEnumerable<Models.Peer> SearchPeers(string query, int limit = 20)
	{
		return _moderationService.SearchPeers(query, limit);
	}
	public Task<Models.Peer> UpdatePeer(Models.Peer peer)
	{
		return _moderationService.UpdatePeer(peer);
	}

	public IAuthzModerationService As(IEnumerable<Claim> claims)
	{
		_moderationService.As(claims);
		return this;
	}
}