using Bogus;
using Letterbook.Adapter.TimescaleFeeds.IntegrationTests.Fixtures;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Letterbook.Adapter.TimescaleFeeds.IntegrationTests;

public class FeedsAdapterTest : IClassFixture<TimescaleDataFixture<FeedsAdapterTest>>
{
	private ITestOutputHelper _output;
	private FeedsAdapter _adapter;
	private FeedsContext _context;
	private FeedsContext _actual;
	private TimescaleDataFixture<FeedsAdapterTest> _timescale;
	private Faker _fake;

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
	[Trait("FeedsAdapter", "Timeline")]
	public void Exists()
	{
		Assert.NotNull(_adapter);
	}

	[Fact]
	public async Task CanQueryAudience()
	{
		var audience = _timescale.Profiles.Take(5).Select(p => Audience.Followers(p));
		var actual = await _adapter.GetTimelineEntries(audience, DateTimeOffset.UtcNow, 100).AnyAsync();

		Assert.True(actual);
	}

	[Fact]
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

	[Fact]
	public async Task CanRemovePost()
	{
		var post = _timescale.Posts.Last();

		await _adapter.RemoveFromAllTimelines(post);

		var actual = await _actual.Timelines.Where(p => p.PostId == post.Id).CountAsync();
		Assert.Equal(0, actual);
	}

	[Fact]
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
}