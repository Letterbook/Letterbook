using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Timescale")]
[Trait("Driver", "Adapter")]
public class FeedsAdapterTest : IClassFixture<TimescaleDataFixture<FeedsAdapterTest>>
{
	private ITestOutputHelper _output;
	private FeedsAdapter _adapter;
	private FeedsContext _context;
	private FeedsContext _actual;
	private TimescaleDataFixture<FeedsAdapterTest> _timescale;
	// private Faker _fake;

	public FeedsAdapterTest(ITestOutputHelper outputHelper, TimescaleDataFixture<FeedsAdapterTest> timescale)
	{
		_timescale = timescale;
		_output = outputHelper;

		_context = _timescale.CreateContext();
		_actual = _timescale.CreateContext();
		_adapter = new FeedsAdapter(_context);

		_output.WriteLine($"Bogus Seed: {_timescale.Seed}");
	}

	[Fact(DisplayName = "Should exist")]
	public void Exists()
	{
		Assert.NotNull(_adapter);
	}

	[Fact(DisplayName = "Should query for a set of audiences")]
	public async Task CanQueryAudience()
	{
		var audience = _timescale.Profiles.Take(5).Select(p => Audience.Followers(p));
		var actual = await _adapter.GetTimelineEntries(audience, DateTimeOffset.UtcNow, 100).AnyAsync();

		Assert.True(actual);
	}

	[Fact(DisplayName = "Should add a post to the correct timelines")]
	public async Task CanAddPost()
	{
		var post = new FakePost("letterbook.example").Generate();
		post.Audience.Remove(Audience.Public);
		post.AddressedTo.Add(Mention.To(_timescale.Profiles[0]));

		// Generated fake audience, plus 1 explicit mention
		var expected = post.Audience.Count + 1;

		_adapter.AddToTimeline(post);
		await _adapter.Commit();

		var actual = await _actual.Timelines.Where(p => p.PostId == post.Id).CountAsync();
		Assert.Equal(expected, actual);
	}

	[Fact(DisplayName = "Should remove a post from all timelines")]
	public async Task CanRemovePost()
	{
		var post = _timescale.Posts.Last();

		await _adapter.RemoveFromAllTimelines(post);

		var actual = await _actual.Timelines.Where(p => p.PostId == post.Id).CountAsync();
		Assert.Equal(0, actual);
	}

	[Fact(DisplayName = "Should remove a post from specific audiences")]
	public async Task CanRemovePostFromAudience()
	{
		var post = _timescale.Posts.First(p => p.AddressedTo.Count >= 1);
		var expected = post.AddressedTo.First();
		post.AddressedTo.Remove(expected);

		await _adapter.RemoveFromTimelines(post, [Audience.FromMention(expected.Subject)]);

		var actual = await _actual.Timelines.Where(p => p.PostId == post.Id).ToListAsync();
		Assert.DoesNotContain(Audience.FromMention(expected.Subject).FediId, actual.Select(p => p.AudienceId));
		Assert.NotEmpty(actual);
	}

	[Fact(DisplayName = "Should update the timeline view of a post")]
	public async Task CanUpdatePost()
	{
		var post = _timescale.Posts.Skip(1).First();
		var expected = $"Updated in {nameof(CanUpdatePost)}";
		post.Preview = expected;

		await _adapter.UpdateTimeline(post);

		var actual = await _actual.Timelines.Where(p => p.PostId == post.Id).Select(p => p.Preview).ToListAsync();
		Assert.All(actual, s => Assert.Equal(s, expected));
		Assert.NotEmpty(actual);
	}
}