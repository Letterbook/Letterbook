using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Medo;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Adapter.Db.Tests;

public class RelationalContextTest : IDisposable
{
	public void Dispose() => _context.Dispose();
	private readonly RelationalContext _context;
	private readonly FakeProfile _profiles;

	public RelationalContextTest()
	{

		var options = new DbOptions
		{
			Host = "localhost",
			Port = "5432",
			Username = "postgres",
			Database = "postgres",
			UseSsl = false,
			Password = "postgres",
		};
		var dbContextOptions = new DbContextOptionsBuilder<RelationalContext>()
			.UseNpgsql(DependencyInjection.DataSource(options))
			.Options;
		_context = new RelationalContext(dbContextOptions);
		_profiles = new FakeProfile("letterbook.example");
	}

	[Fact]
	public void CanTrackAccounts()
	{
		var account = new FakeAccount().Generate();
		_context.Add(account);
	}

	[Fact]
	public void CanTrackProfiles()
	{
		var profile = new FakeProfile().Generate();
		_context.Add(profile);
	}

	[Fact]
	public void CanTrackFollows()
	{
		var profile = new FakeProfile("letterbook.example").Generate();
		var follower = new FakeProfile().Generate();
		profile.AddFollower(follower, FollowState.Accepted);

		_context.Add(profile);
	}

	[Fact]
	public void CanTrackReports()
	{
		ModerationReportId id = Uuid7.NewUuid7();
		ThreadId threadId = Uuid7.NewUuid7();
		var policy = new ModerationPolicy
		{
			Id = 10,
			Created = DateTimeOffset.UtcNow,
			Title = "test policy",
			Summary = "this policy is for test purposes",
			Policy = "this is the full text of the policy, which might normally be multiple paragraphs",
		};
		var profile = _profiles.Generate();
		var reporter = _profiles.Generate();
		var post = new FakePost(profile).Generate();
		var moderator = new FakeAccount(false);
		var report = new ModerationReport
		{
			FediId = new Uri($"https://letterbook.example/report/{id}"),
			Summary = "test report",
			Moderators = [moderator],
			Reporter = reporter,
			RelatedPosts = [post],
			Subjects = [post.Creators.First()],
			Policies = [policy],
			Context = new ThreadContext
			{
				Id = threadId,
				FediId = new Uri($"https://letterbook.example/thread/{threadId}"),
				RootId = default
			}
		};

		_context.Add(report);
	}
}