using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class TimelineServiceTest : WithMocks
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly Mock<ILogger<TimelineService>> _logger;
    private readonly CoreOptions _opts;
    private readonly Mock<IFeedsAdapter> _feeds;
    private readonly FakeNote _note;
    private readonly FakeProfile _profile;
    private TimelineService _timeline;

    public TimelineServiceTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _logger = new Mock<ILogger<TimelineService>>();
        _feeds = new Mock<IFeedsAdapter>();
        _timeline = new TimelineService(_logger.Object, CoreOptionsMock, _feeds.Object, AccountProfileMock.Object);

        _opts = CoreOptionsMock.Value;
        _note = new FakeNote();
        _profile = new FakeProfile(_opts.DomainName);
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_timeline);
    }

    [Fact]
    public void AddToPublicOnCreate()
    {
        var note = _note.Generate();
        note.Visibility.Add(Audience.Public);

        _timeline.HandleCreate(note);
        
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.Is<ICollection<Audience>>(audience => audience.Contains(Audience.Public)), It.IsAny<Profile>()), Times.Exactly(1));
    }
    
    [Fact]
    public void AddToFollowersOnCreate()
    {
        var note = _note.Generate();
        var expected = Audience.FromFollowers(note.Creators.First());
        note.Visibility.Add(expected);

        _timeline.HandleCreate(note);
        
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()), Times.Exactly(1));
    }

    [Fact]
    public void AddToFollowersImplicitlyOnCreate()
    {
        var note = _note.Generate();
        var expected = Audience.FromFollowers(note.Creators.First());
        note.Visibility.Add(Audience.Public);

        _timeline.HandleCreate(note);
        
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()), Times.Exactly(1));
    }

    [Fact]
    public void AddToMentionsOnCreate()
    {
        var note = _note.Generate();
        var mention = new Mention()
        {
            Id = Guid.NewGuid(),
            Subject = _profile.Generate(),
            Visibility = MentionVisibility.To
        };
        note.Mentions.Add(mention);
        var expected = Audience.FromMention(mention.Subject);

        _timeline.HandleCreate(note);
        
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()), Times.Exactly(1));
    }
}