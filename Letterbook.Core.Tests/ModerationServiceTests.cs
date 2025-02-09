using Letterbook.Core.Exceptions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using MockQueryable;
using Moq;

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

	public ModerationServiceTests()
	{
		_service = new ModerationService(DataAdapterMock.Object, AuthorizationServiceMock.Object, ProfileEventServiceMock.Object, AccountServiceMock.Object);
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
			new() { Id = 1, Title = "Policy 1" }
		];

		DataAdapterMock.Setup(m => m.Profiles(It.IsAny<ProfileId[]>())).Returns(_profiles.BuildMock());
		DataAdapterMock.Setup(m => m.Accounts(It.IsAny<Guid[]>())).Returns(_accounts.BuildMock());
		DataAdapterMock.Setup(m => m.Posts(It.IsAny<PostId[]>())).Returns(_posts.BuildMock());
		DataAdapterMock.Setup(m => m.ModerationReports(It.IsAny<ModerationReportId[]>())).Returns(_reports.BuildMock());

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_service);
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
		Assert.Equal(actual.Remarks.First(), remark);
	}

	[Fact(DisplayName = "Should assign a moderator")]
	public async Task ShouldAssign()
	{
		var mod = _fakeAccounts.Generate();
		DataAdapterMock.Setup(m => m.LookupAccount(mod.Id)).ReturnsAsync(mod);
		var actual = await _service.As([]).AssignModerator(_reports[0].Id, mod.Id);

		Assert.Single(actual.Moderators);
		Assert.Equal(actual.Moderators.First(), mod);
	}

	[Fact(DisplayName = "Should remove an assigned moderator")]
	public async Task ShouldUnassign()
	{
		var mod = _fakeAccounts.Generate();
		_reports[0].Moderators.Add(mod);
		DataAdapterMock.Setup(m => m.LookupAccount(mod.Id)).ReturnsAsync(mod);

		var actual = await _service.As([]).AssignModerator(_reports[0].Id, mod.Id, true);

		Assert.Empty(actual.Moderators);
	}

	[Fact(DisplayName = "Should close open reports")]
	public async Task ShouldClose()
	{
		var actual = await _service.As([]).CloseReport(_reports[0].Id);

		Assert.NotEqual(DateTimeOffset.MaxValue, actual.Closed);
	}

	[Fact(DisplayName = "Should reopen closed reports")]
	public async Task ShouldReopen()
	{
		var actual = await _service.As([]).CloseReport(_reports[0].Id, true);

		Assert.Equal(DateTimeOffset.MaxValue, actual.Closed);
	}

	[Fact(DisplayName = "Should not modify already closed reports")]
	public async Task ShouldNotDoubleClose()
	{
		var expected = DateTimeOffset.UtcNow.AddHours(-1);
		_reports[0].Closed = expected;
		var actual = await _service.As([]).CloseReport(_reports[0].Id);

		Assert.Equal(expected, actual.Closed);
	}

	[Fact(DisplayName = "Should immediately close reports with a future close date")]
	public async Task ShouldClosePendingClosed()
	{
		var expected = DateTimeOffset.UtcNow.AddHours(1);
		_reports[0].Closed = expected;
		var actual = await _service.As([]).CloseReport(_reports[0].Id);

		Assert.True(actual.Closed <= DateTimeOffset.UtcNow);
	}

	[Fact(DisplayName = "Should update assigned Moderators")]
	public async Task ShouldUpdateMods()
	{
		var given = new FakeReport(_profiles[2], _profiles[0], _opts).Generate();
		given.Id = _reports[0].Id;
		var mod = _fakeAccounts.Generate();
		given.Moderators = [mod];
		DataAdapterMock.Setup(m => m.Accounts(mod.Id)).Returns(given.Moderators.BuildMock());

		var actual = await _service.As([]).UpdateReport(given.Id, given);

		Assert.Equivalent(given.Moderators, actual.Moderators);
	}

	[Fact(DisplayName = "Should update assigned Moderators")]
	public async Task ShouldUpdatePolicies()
	{
		var given = new FakeReport(_profiles[2], _profiles[0], _opts).Generate();
		given.Id = _reports[0].Id;
		given.Policies = [_policies[1]];

		var actual = await _service.As([]).UpdateReport(given.Id, given);

		Assert.Equivalent(given.Policies, actual.Policies);
	}

	[Fact(DisplayName = "Should not update summary, subjects, or posts")]
	public async Task ShouldNotUpdateOtherProperties()
	{
		var given = new FakeReport(_profiles[0], _profiles[1], _opts).Generate();
		given.RelatedPosts = _posts;
		given.Id = _reports[0].Id;

		var actual = await _service.As([]).UpdateReport(given.Id, given);

		Assert.NotEqual(actual.Summary, given.Summary);
		Assert.NotEqual(actual.Reporter, given.Reporter);
		Assert.NotEqual(actual.Subjects, given.Subjects);
		Assert.NotEqual(actual.RelatedPosts, given.RelatedPosts);
		Assert.NotEqual(actual.Created, given.Created);
	}
}