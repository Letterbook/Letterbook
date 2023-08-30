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
        _note = new FakeNote().Generate();
        _fake = new Faker();
    }
    
    [Fact(DisplayName = "Should exist")]
    [Trait("FeedsAdapter", "Timeline")]
    public void Exists()
    {
        Assert.NotNull(_adapter);
    }
    
    [Fact(DisplayName = "Should add a content item to a single timeline")]
    [Trait("FeedsAdapter", "Timeline")]
    public async void AddToOneTimeline()
    {
        using (_adapter)
        {
            await _adapter.AddToTimeline(_note, Audience.FromFollowers(_note.Creators.First()));
        }

        var results = _actual.Feeds.Where(r => r.EntityId == _note.Id.ToString()).AsEnumerable();
        Assert.Single(results);
    }

    [Fact(DisplayName = "Should add a content item to every relevant timeline")]
    [Trait("FeedsAdapter", "Timeline")]
    public async void AddToManyTimelines()
    {
        var audiences = Enumerable.Range(0, 5)
            .Select(i => _fake.Internet.UrlWithPath())
            .ToArray();
        _note.Visibility.Clear();
        _note.Visibility.UnionWith(audiences.Select(u => Audience.FromUri(new Uri(u))));

        using (_adapter)
        {
            await _adapter.AddToTimeline(_note, _note.Visibility);
        }

        var actual = _actual.Feeds.Where(r => r.EntityId == _note.Id.ToString()).AsEnumerable();
        Assert.Equal(audiences, actual.Select(e => e.AudienceKey));
    }
}