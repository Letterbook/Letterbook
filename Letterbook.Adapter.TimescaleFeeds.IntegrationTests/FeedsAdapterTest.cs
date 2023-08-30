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
            await _adapter.Commit();
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
            await _adapter.Commit();
        }

        var actual = _actual.Feeds.Where(r => r.EntityId == _note.Id.ToString()).AsEnumerable();
        Assert.Equal(audiences, actual.Select(e => e.AudienceKey));
    }

    [Fact(DisplayName = "Should delete from all timelines")]
    [Trait("FeedsAdapter", "Timeline")]
    public async void DeleteFromAllTimelines()
    {
        var audiences = Enumerable.Range(0, 5)
            .Select(i => _fake.Internet.UrlWithPath())
            .ToArray();
        _note.Visibility.Clear();
        _note.Visibility.UnionWith(audiences.Select(u => Audience.FromUri(new Uri(u))));

        var arrange = new FeedsAdapter(_actual);
        await arrange.AddToTimeline(_note, _note.Visibility);
        await arrange.Commit();

        using (_adapter)
        {
            await _adapter.RemoveFromTimelines(_note);
            await _adapter.Commit();
        }

        var actual = _actual.Feeds.Where(t => t.EntityId == _note.Id.ToString()).AsEnumerable();
        Assert.Empty(actual);
    }
    
    [Fact(DisplayName = "Should delete from specified timelines")]
    [Trait("FeedsAdapter", "Timeline")]
    public async void DeleteFromSelectTimelines()
    {
        var audiences = Enumerable.Range(0, 5)
            .Select(i => _fake.Internet.UrlWithPath())
            .ToArray();
        _note.Visibility.Clear();
        _note.Visibility.UnionWith(audiences.Select(u => Audience.FromUri(new Uri(u))));

        var arrange = new FeedsAdapter(_actual);
        await arrange.AddToTimeline(_note, _note.Visibility);
        await arrange.Commit();

        using (_adapter)
        {
            await _adapter.RemoveFromTimelines(_note, _note.Visibility.Take(2).ToList());
            await _adapter.Commit();
        }

        var actual = _actual.Feeds.Where(t => t.EntityId == _note.Id.ToString()).AsEnumerable();
        Assert.Equal(3, actual.Count());
    }

    [Fact(DisplayName = "Should query timelines")]
    [Trait("FeedsAdapter", "Timeline")]
    public async void QueryTimelines()
    {
        var booster = new FakeProfile("https://letterbook.example");
        var boostAudience = Audience.FromBoost(booster);
        var notes = new FakeNote().Generate(3);
        var arrange = new FeedsAdapter(_actual);
        foreach (var n in notes)
        {
            n.Visibility.Add(Audience.Public);
            n.Visibility.Add(boostAudience);
            await arrange.AddToTimeline(n, n.Visibility);
        }
        var following = new HashSet<Audience> { boostAudience, Audience.Public };
        await arrange.Commit();
        
        var actual = _adapter.GetTimelineEntries(following, DateTime.UtcNow, 20).AsEnumerable();
        
        Assert.Equal(3, actual.Count());
    }
}