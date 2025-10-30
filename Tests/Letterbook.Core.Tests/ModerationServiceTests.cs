using System.Security.Claims;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class ModerationServiceTests : WithMocks
{
	private readonly ModerationService _service;
	private readonly FakeAccount _fakeAccounts;
	private readonly FakeProfile _fakeProfiles;
	private readonly List<Profile> _profiles;
	private readonly List<Post> _posts;
	private readonly CoreOptions _opts;
	private readonly List<ModerationReport> _reports;
	private readonly List<Account> _accounts;
	private readonly List<ModerationPolicy> _policies;

	public ModerationServiceTests(ITestOutputHelper output)
	{
		_service = new ModerationService(Mock.Of<ILogger<ModerationService>>(), DataAdapterMock.Object, AuthorizationServiceMock.Object, ProfileEventServiceMock.Object,
			AccountServiceMock.Object, ModerationEventPublisherMock.Object, ActivityPublisherMock.Object, ApCrawlerSchedulerMock.Object);
		_fakeAccounts = new FakeAccount();
		_fakeProfiles = new FakeProfile();
		_accounts = new FakeAccount().Generate(3);
		_profiles = _fakeProfiles.Generate(3);
		_posts = new FakePost(_profiles[0]).Generate(3);
		_opts = new CoreOptions();
		_reports = new FakeReport(_profiles[2], _profiles[0], _opts).Generate(1);
		_policies =
		[
			new() { Id = 0, Title = "Policy 0" },
			new() { Id = 1, Title = "Policy 1" },
			new() { Id = 2, Title = "Retired", Retired = DateTimeOffset.UtcNow.AddHours(-1) }
		];

		DataAdapterMock.Setup(m => m.Policies(It.IsAny<ModerationPolicyId[]>())).Returns(_policies.BuildMock());
		DataAdapterMock.Setup(m => m.AllPolicies()).Returns(_policies.BuildMock());
		DataAdapterMock.Setup(m => m.Profiles(It.IsAny<ProfileId[]>())).Returns(_profiles.BuildMock());
		DataAdapterMock.Setup(m => m.Accounts(It.IsAny<Guid[]>())).Returns(_accounts.BuildMock());
		DataAdapterMock.Setup(m => m.Posts(It.IsAny<PostId[]>())).Returns(_posts.BuildMock());
		DataAdapterMock.Setup(m => m.ModerationReports(It.IsAny<ModerationReportId[]>())).Returns(_reports.BuildMock());
		DataAdapterMock.Setup(m => m.Peers(It.IsAny<Uri[]>())).Returns(new List<Peer> { new(_profiles[0].FediId) }.BuildMock());

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_service);
	}

	[Fact(DisplayName = "Should lookup a report by ID")]
	public async Task CanLookup()
	{
		var actual = await _service.As([]).LookupReport(_reports[0].Id);

		Assert.Equivalent(_reports[0], actual);
		ModerationEventPublisherMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should create new reports")]
	public async Task CanCreate()
	{
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = _profiles[2],
			Subjects = [_profiles[0]],
			RelatedPosts = _posts
		};
		var actual = await _service.As([]).CreateReport(_profiles[2].Id, report);

		Assert.NotNull(actual);
		ProfileEventServiceMock.Verify(m => m.Reported(_profiles[0], _profiles[2]), Times.Once());
		ModerationEventPublisherMock.Verify(m => m.Created(It.IsAny<ModerationReport>(), It.IsAny<ProfileId>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
	}

	[Fact(DisplayName = "Should forward new reports")]
	public async Task CanForward()
	{
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = _profiles[2],
			Subjects = [_profiles[0]],
			RelatedPosts = _posts,
			Forwarded = [_profiles[0].SharedInbox!]
		};
		var actual = await _service.As([]).CreateReport(_profiles[2].Id, report);

		Assert.NotNull(actual);
		ProfileEventServiceMock.Verify(m => m.Reported(_profiles[0], _profiles[2]), Times.Once());
		ModerationEventPublisherMock.Verify(m => m.Created(It.IsAny<ModerationReport>(), It.IsAny<ProfileId>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
		ActivityPublisherMock.Verify(m => m.Report(_profiles[0].SharedInbox!, It.IsAny<ModerationReport>(), It.IsAny<IEnumerable<Claim>>(), It.IsAny<ModerationReport.Scope>()));
	}

	[Fact(DisplayName = "Should require at least one subject in new reports")]
	public async Task ShouldRequireSubject()
	{
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = _profiles[2],
			RelatedPosts = _posts
		};

		await Assert.ThrowsAsync<CoreException>(async () => await _service.As([]).CreateReport(_profiles[2].Id, report));
	}

	[Fact(DisplayName = "Should require known subjects in new reports")]
	public async Task ShouldRequireKnownSubject()
	{
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = _profiles[2],
			Subjects = [_fakeProfiles.Generate()]
		};

		await Assert.ThrowsAsync<CoreException>(async () => await _service.As([]).CreateReport(_profiles[2].Id, report));
	}

	[Fact(DisplayName = "Should require known posts in new reports")]
	public async Task ShouldRequireKnownPost()
	{
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = _profiles[2],
			Subjects = [_profiles[0]],
			RelatedPosts = new FakePost(_profiles[2]).Generate(2)
		};

		await Assert.ThrowsAsync<CoreException>(async () => await _service.As([]).CreateReport(_profiles[2].Id, report));
	}

	[Fact(DisplayName = "Should receive inbound federated reports")]
	public async Task CanReceive()
	{
		var actor = new FakeProfile("peer.example").Generate();
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = actor,
			Subjects = [_profiles[0]],
			RelatedPosts = _posts
		};
		DataAdapterMock.Setup(m => m.Profiles(It.Is<Uri[]>(uris => uris.Contains(actor.FediId)))).Returns(new List<Profile>() { actor }.BuildMock());
		DataAdapterMock.Setup(m => m.Profiles(It.IsAny<Uri[]>())).Returns(new List<Profile>() { _profiles[0] }.BuildMock());
		DataAdapterMock.Setup(m => m.Posts(It.IsAny<Uri[]>())).Returns(_posts.BuildMock());
		var actual = await _service.As([]).ReceiveReport(actor.FediId, report);

		Assert.NotNull(actual);
		ProfileEventServiceMock.Verify(m => m.Reported(_profiles[0], default), Times.Once());
		ModerationEventPublisherMock.Verify(m => m.Created(It.IsAny<ModerationReport>(), It.IsAny<ProfileId>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
	}

	[Fact(DisplayName = "Should crawl unknown objects from received federated reports")]
	public async Task CanReceive_CrawlUnknownObjects()
	{
		var actor = new FakeProfile("peer.example").Generate();
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = actor,
			Subjects = [_profiles[0]],
			RelatedPosts = _posts
		};
		DataAdapterMock.Setup(m => m.Profiles(It.Is<Uri[]>(uris => uris.Contains(actor.FediId)))).Returns(new List<Profile>() { actor }.BuildMock());
		DataAdapterMock.Setup(m => m.Profiles(It.IsAny<Uri[]>())).Returns(new List<Profile>() { _profiles[0] }.BuildMock());
		DataAdapterMock.Setup(m => m.Posts(It.IsAny<Uri[]>())).Returns(_posts.Take(1).BuildMock());
		var actual = await _service.As([]).ReceiveReport(actor.FediId, report);

		Assert.NotNull(actual);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), It.IsAny<Uri>(), 1), Times.Exactly(2));
	}

	[Fact(DisplayName = "Should crawl unknown actors from received federated reports")]
	public async Task CanReceive_CrawlUnknownReporter()
	{
		var actor = new FakeProfile("peer.example").Generate();
		var report = new ModerationReport(_opts, "test report")
		{
			Reporter = actor,
			Subjects = [_profiles[0]],
			RelatedPosts = _posts
		};
		DataAdapterMock.Setup(m => m.Profiles(It.Is<Uri[]>(uris => uris.Contains(actor.FediId)))).Returns(new List<Profile>().BuildMock());
		DataAdapterMock.Setup(m => m.Profiles(It.Is<Uri[]>(uris => uris.Length > 1))).Returns(new List<Profile>() { _profiles[0] }.BuildMock());
		DataAdapterMock.Setup(m => m.Posts(It.IsAny<Uri[]>())).Returns(_posts.BuildMock());
		var actual = await _service.As([]).ReceiveReport(actor.FediId, report);

		Assert.NotNull(actual);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlProfile(It.IsAny<ProfileId>(), It.IsAny<Uri>(), 0), Times.Once);
	}

	[Fact(DisplayName = "Should add a remark")]
	public async Task ShouldAddRemark()
	{
		var remark = new ModerationRemark
		{
			Report = _reports[0],
			Author = _fakeAccounts.Generate(),
			Created = DateTimeOffset.UtcNow,
			Updated = DateTimeOffset.UtcNow,
			Text = "test remark"
		};

		var actual = await _service.As([]).AddRemark(_reports[0].Id, remark);

		Assert.Single(actual.Remarks);
		Assert.Equal(remark, actual.Remarks.First());
		ModerationEventPublisherMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should assign a moderator")]
	public async Task ShouldAssign()
	{
		var mod = _fakeAccounts.Generate();
		DataAdapterMock.Setup(m => m.LookupAccount(mod.Id)).ReturnsAsync(mod);
		var actual = await _service.As([]).AssignModerator(_reports[0].Id, mod.Id);

		Assert.Single(actual.Moderators);
		Assert.Equal(actual.Moderators.First(), mod);
		ModerationEventPublisherMock.Verify(m => m.Assigned(It.IsAny<ModerationReport>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
	}

	[Fact(DisplayName = "Should remove an assigned moderator")]
	public async Task ShouldUnassign()
	{
		var mod = _fakeAccounts.Generate();
		_reports[0].Moderators.Add(mod);
		DataAdapterMock.Setup(m => m.LookupAccount(mod.Id)).ReturnsAsync(mod);

		var actual = await _service.As([]).AssignModerator(_reports[0].Id, mod.Id, true);

		Assert.Empty(actual.Moderators);
		ModerationEventPublisherMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should close open reports")]
	public async Task ShouldClose()
	{
		var actual = await _service.As([]).CloseReport(_reports[0].Id, _accounts[0].Id);

		Assert.NotEqual(DateTimeOffset.MaxValue, actual.Closed);
		ModerationEventPublisherMock.Verify(m => m.Closed(It.IsAny<ModerationReport>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);

	}

	[Fact(DisplayName = "Should reopen closed reports")]
	public async Task ShouldReopen()
	{
		var actual = await _service.As([]).CloseReport(_reports[0].Id, _accounts[0].Id, true);

		Assert.Equal(DateTimeOffset.MaxValue, actual.Closed);
		ModerationEventPublisherMock.Verify(m => m.Reopened(It.IsAny<ModerationReport>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
	}

	[Fact(DisplayName = "Should not modify closing date of already closed reports")]
	public async Task ShouldNotDoubleClose()
	{
		var expected = DateTimeOffset.UtcNow.AddHours(-1);
		_reports[0].Closed = expected;
		var actual = await _service.As([]).CloseReport(_reports[0].Id, _accounts[0].Id);

		Assert.Equal(expected, actual.Closed);
		ModerationEventPublisherMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should immediately close reports with a future close date")]
	public async Task ShouldClosePendingClosed()
	{
		var expected = DateTimeOffset.UtcNow.AddHours(1);
		_reports[0].Closed = expected;
		var actual = await _service.As([]).CloseReport(_reports[0].Id, _accounts[0].Id);

		Assert.True(actual.Closed <= DateTimeOffset.UtcNow);
		ModerationEventPublisherMock.Verify(m => m.Closed(It.IsAny<ModerationReport>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);

	}

	[Fact(DisplayName = "Should update assigned Moderators")]
	public async Task ShouldUpdateMods()
	{
		var given = new FakeReport(_profiles[2], _profiles[0], _opts).Generate();
		given.Id = _reports[0].Id;
		var mod = _fakeAccounts.Generate();
		given.Moderators = [mod];
		DataAdapterMock.Setup(m => m.Accounts(mod.Id)).Returns(given.Moderators.BuildMock());

		var actual = await _service.As([]).UpdateReport(given.Id, given, mod.Id);

		Assert.Equivalent(given.Moderators, actual.Moderators);
		ModerationEventPublisherMock.Verify(m => m.Assigned(It.IsAny<ModerationReport>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Claim>>()), Times.Once);
	}

	[Fact(DisplayName = "Should update associated Policies")]
	public async Task ShouldUpdatePolicies()
	{
		var given = new FakeReport(_profiles[2], _profiles[0], _opts).Generate();
		given.Id = _reports[0].Id;
		given.Policies = [_policies[1]];

		var actual = await _service.As([]).UpdateReport(given.Id, given, Guid.NewGuid());

		Assert.Equivalent(given.Policies, actual.Policies);
	}

	[Fact(DisplayName = "Should forward to new recipients")]
	public async Task ShouldUpdateForwarding()
	{
		var given = new FakeReport(_profiles[2], _profiles[0], _opts).Generate();
		given.Id = _reports[0].Id;
		given.Forwarded = [_profiles[0].SharedInbox!];

		var actual = await _service.As([]).UpdateReport(given.Id, given, Guid.NewGuid());

		Assert.Equivalent(given.Policies, actual.Policies);
		ActivityPublisherMock.Verify(m => m.Report(_profiles[0].SharedInbox!, It.IsAny<ModerationReport>(), It.IsAny<IEnumerable<Claim>>(), It.IsAny<ModerationReport.Scope>()));
	}

	[Fact(DisplayName = "Should not forward to unrelated recipients")]
	public async Task ShouldNotForwardUnrelated()
	{
		var given = new FakeReport(_profiles[2], _profiles[0], _opts).Generate();
		given.Id = _reports[0].Id;
		given.Forwarded = [new Uri("https://inbox.example")];

		var actual = await _service.As([]).UpdateReport(given.Id, given, Guid.NewGuid());

		Assert.Equivalent(given.Policies, actual.Policies);
		ActivityPublisherMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should not repeat forwarding")]
	public async Task ShouldNotForwardRepeat()
	{
		_reports[0].Forwarded.Add(_profiles[0].SharedInbox!);
		var given = new FakeReport(_profiles[2], _profiles[0], _opts).Generate();
		given.Id = _reports[0].Id;
		given.Forwarded = [_profiles[0].SharedInbox!];

		var actual = await _service.As([]).UpdateReport(given.Id, given, Guid.NewGuid());

		Assert.Equivalent(given.Policies, actual.Policies);
		ActivityPublisherMock.VerifyNoOtherCalls();
	}

	[Fact(DisplayName = "Should not update summary, subjects, or posts")]
	public async Task ShouldNotUpdateOtherProperties()
	{
		var given = new FakeReport(_profiles[0], _profiles[1], _opts).Generate();
		given.RelatedPosts = _posts;
		given.Id = _reports[0].Id;

		var actual = await _service.As([]).UpdateReport(given.Id, given, Guid.NewGuid());

		Assert.NotEqual(actual.Summary, given.Summary);
		Assert.NotEqual(actual.Reporter, given.Reporter);
		Assert.NotEqual(actual.Subjects, given.Subjects);
		Assert.NotEqual(actual.RelatedPosts, given.RelatedPosts);
		Assert.NotEqual(actual.Created, given.Created);
	}

	[InlineData(false, 2)]
	[InlineData(true, 3)]
	[Theory(DisplayName = "Should list policies")]
	public async Task ShouldListPolicies(bool expired, int expected)
	{
		var actual = await _service.As([]).ListPolicies(expired).ToListAsync();

		Assert.Equivalent(_policies.Take(expected), actual);
	}

	[Fact(DisplayName = "Should add a new policy")]
	public async Task ShouldAddNewPolicy()
	{
		var given = new ModerationPolicy
		{
			Id = 10,
			Title = "new policy",
			Summary = "A short summary",
			Policy = "A long, detailed, policy document.",
		};

		var actual = await _service.As([]).AddPolicy(given);

		Assert.Equal(given, actual);
	}

	[Fact(DisplayName = "Should retire an existing policy")]
	public async Task ShouldRetirePolicy()
	{
		var actual = await _service.As([]).RetirePolicy(_policies[0].Id);

		Assert.True(actual.Retired <= DateTimeOffset.UtcNow);
	}

	[Fact(DisplayName = "Should restore a retired policy")]
	public async Task ShouldRestorePolicy()
	{
		var actual = await _service.As([]).RetirePolicy(_policies[2].Id, true);

		Assert.True(actual.Retired > DateTimeOffset.UtcNow);
	}

	[Fact(DisplayName = "Should init a new peer")]
	public async Task ShouldInit()
	{
		DataAdapterMock.Setup(m => m.Peers(It.IsAny<Uri[]>())).Returns(new List<Peer>().BuildMock());
		var peer = new Uri("https://peer.example");
		var actual = await _service.As([]).GetOrInitPeerRestrictions(peer);

		Assert.NotNull(actual);
		DataAdapterMock.Verify(m => m.Add(It.IsAny<Peer>()));
		DataAdapterMock.Verify(m => m.Commit());
	}
}