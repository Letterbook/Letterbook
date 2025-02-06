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

	public ModerationServiceTests()
	{
		_service = new ModerationService(DataAdapterMock.Object, AuthorizationServiceMock.Object, ProfileEventServiceMock.Object);
		_fakeAccounts = new FakeAccount();
		_fakeProfiles = new FakeProfile();
		_profiles = _fakeProfiles.Generate(3);
		_posts = new FakePost(_profiles[0]).Generate(3);
		_opts = new CoreOptions();

		DataAdapterMock.Setup(m => m.Profiles(It.IsAny<ProfileId[]>())).Returns(_profiles.BuildMock());
		DataAdapterMock.Setup(m => m.Posts(It.IsAny<PostId[]>())).Returns(_posts.BuildMock());

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

}