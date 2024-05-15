using Bogus;
using Letterbook.Adapter.TimescaleFeeds.IntegrationTests.Fixtures;
using Letterbook.Core.Models;
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
}