using Bogus;
using Letterbook.Adapter.TimescaleFeeds.IntegrationTests.Fixtures;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Xunit.Abstractions;

namespace Letterbook.Adapter.TimescaleFeeds.IntegrationTests;

public class FeedsAdapterTest : IClassFixture<TimescaleFixture>
{
	private ITestOutputHelper _output;
	private FeedsAdapter _adapter;
	private FeedsContext _context;
	private FeedsContext _actual;
	private Note _note;
	private TimescaleFixture _timescale;
	private Faker _fake;

	public FeedsAdapterTest(ITestOutputHelper outputHelper, TimescaleFixture timescale)
	{
		_timescale = timescale;
		_output = outputHelper;

		_context = _timescale.CreateContext();
		_actual = _timescale.CreateContext();
		_adapter = new FeedsAdapter(_context);

		_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
	}

	[Fact(DisplayName = "Should exist")]
	[Trait("FeedsAdapter", "Timeline")]
	public void Exists()
	{
		Assert.NotNull(_adapter);
	}
}