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
    private Note _note;
    private TimescaleFixture _fixture;
    
    public FeedsAdapterTest(ITestOutputHelper outputHelper, TimescaleFixture fixture)
    {
        _fixture = fixture;
        _output = outputHelper;

        _context = _fixture.CreateContext();
        _adapter = new FeedsAdapter(_context);

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _note = new FakeNote().Generate();
    }
    
    [Fact(DisplayName = "Should exist")]
    [Trait("FeedsAdapter", "Timeline")]
    public void Exists()
    {
        Assert.NotNull(_adapter);
    }
    
    [Fact(DisplayName = "Should add a content item to a single timeline")]
    [Trait("FeedsAdapter", "Timeline")]
    public void AddToOneTimeline()
    {
        using (_adapter)
        { 
            _adapter.AddToTimeline(_note, Audience.FromFollowers(_note.Creators.First()));
        }
    }
}