using Bogus;
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

        outputHelper.WriteLine($"Bogus Seed: {Init.WithSeed()}");
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
    [Trait("Category", "OnCreate")]
    public void AddToPublicOnCreate()
    {
        var note = _note.Generate();
        note.Visibility.Add(Audience.Public);

        _timeline.HandleCreate(note);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(Audience.Public)), It.IsAny<Profile>()),
            Times.Exactly(1));
    }

    [Fact]
    [Trait("Category", "OnCreate")]
    public void AddToFollowersOnCreate()
    {
        var note = _note.Generate();
        var expected = Audience.FromFollowers(note.Creators.First());
        note.Visibility.Add(expected);

        _timeline.HandleCreate(note);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Exactly(1));
    }

    [Fact]
    [Trait("Category", "OnCreate")]
    public void AddToFollowersImplicitlyOnCreate()
    {
        var note = _note.Generate();
        var expected = Audience.FromFollowers(note.Creators.First());
        note.Visibility.Add(Audience.Public);

        _timeline.HandleCreate(note);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Exactly(1));
    }

    [Fact]
    [Trait("Category", "OnCreate")]
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

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Exactly(1));
    }

    [Fact]
    [Trait("Category", "OnCreate")]
    public void AddMentionToNotificationsOnCreate()
    {
        var note = _note.Generate();
        var profile = _profile.Generate();
        note.Mentions.Add(new Mention()
        {
            Id = Guid.NewGuid(),
            Subject = profile,
            Visibility = MentionVisibility.To
        });

        _timeline.HandleCreate(note);

        _feeds.Verify(m => m.AddNotification<Note>(profile, note, note.Creators, ActivityType.Create),
            Times.Exactly(1));
    }

    [Fact]
    [Trait("Category", "OnCreate")]
    public void AddAllMentionsToNotificationsOnCreate()
    {
        var faker = new Faker();
        var note = _note.Generate();
        var mentions = _profile.Generate(3).Select(p => new Mention()
        {
            Id = Guid.NewGuid(),
            Subject = p,
            Visibility = faker.PickRandom<MentionVisibility>()
        }).ToArray();
        foreach (var mention in mentions)
        {
            note.Mentions.Add(mention);
        }

        _timeline.HandleCreate(note);

        foreach (var mention in mentions)
        {
            _feeds.Verify(m => m.AddNotification(mention.Subject, note, note.Creators, ActivityType.Create), Times.Exactly(1));
        }
    }

}