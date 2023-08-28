using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Adapter.TimescaleFeeds._Tests;

public class FeedsAdapterTest
{
    private ITestOutputHelper _output;
    private FeedsAdapter _adapter;
    private Mock<FeedsContext> _feedsContext;
    private IOptions<FeedsDbOptions> _options;
    private Note _note;
    
    public FeedsAdapterTest(ITestOutputHelper outputHelper)
    {
        _output = outputHelper;
        _options = Options.Create(new FeedsDbOptions());
        _feedsContext = new Mock<FeedsContext>(_options);
        _feedsContext.SetupAllProperties();

        _adapter = new FeedsAdapter(_feedsContext.Object);

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
        _adapter.AddToTimeline(_note, Audience.FromFollowers(_note.Creators.First()));
        
        _feedsContext.Verify(m => m.Feeds.FromSql(It.IsAny<FormattableString>()), Times.Once);
        _feedsContext.Verify(m => m.Feeds.FromSql(It.IsAny<FormattableString>()), Times.Never);
    }
}