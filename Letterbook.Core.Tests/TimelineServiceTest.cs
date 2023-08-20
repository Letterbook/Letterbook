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

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add public posts to the public audience")]
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

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add follower posts to the creator's follower audience")]
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

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add public posts to the creator's follower audience")]
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

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add any posts to the mentioned profiles' feeds")]
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

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add any posts to the mentioned profile's notifications")]
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

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add any posts to all of the mentioned profiles' notifications")]
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
            _feeds.Verify(m => m.AddNotification(mention.Subject, note, note.Creators, ActivityType.Create),
                Times.Exactly(1));
        }
    }

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should not add private posts to the public or follower feeds")]
    public void NoAddPrivateOnCreate()
    {
        var note = _note.Generate();
        note.Visibility.Remove(Audience.Public);
        note.Visibility.Remove(Audience.FromFollowers(note.Creators.First()));
        var mentioned = Mention.To(_profile.Generate());
        note.Mentions.Add(mentioned);
        
        _timeline.HandleCreate(note);

        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), Audience.Public, It.IsAny<Profile>()), Times.Never);
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(), Audience.FromFollowers(note.Creators.First()), It.IsAny<Profile>()),
            Times.Never);
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(Audience.Public)), It.IsAny<Profile>()),
            Times.Never);
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience =>
                    audience.Contains(Audience.FromFollowers(note.Creators.First()))), It.IsAny<Profile>()),
            Times.Never);
    }

    [Trait("TimelineService", "HandleBoost")]
    [Fact(DisplayName = "HandleBoost should add public posts to the boost feed")]
    public void AddPublicToTimelineOnBoost()
    {
        var note = _note.Generate();
        note.Visibility.Add(Audience.Public);
        var booster = _profile.Generate();
        note.BoostedBy.Add(booster);

        _timeline.HandleBoost(note);

        _feeds.Verify(m => m.AddToTimeline(note, Audience.FromBoost(booster), booster), Times.Exactly(1));
    }
    
    [Trait("TimelineService", "HandleBoost")]
    [Fact(DisplayName = "HandleBoost should not add follower posts to any feed")]
    public void NoAddFollowersToTimelineOnBoost()
    {
        var note = _note.Generate();
        note.Visibility.Add(Audience.FromFollowers(note.Creators.First()));
        var booster = _profile.Generate();
        note.BoostedBy.Add(booster);

        _timeline.HandleBoost(note);

        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<Audience>(), It.IsAny<Profile>()), Times.Never);
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<ICollection<Audience>>(), It.IsAny<Profile>()), Times.Never);
    }
    
    [Trait("TimelineService", "HandleBoost")]
    [Fact(DisplayName = "HandleBoost should not add private posts to any feed")]
    public void NoAddPrivateToTimelineOnBoost()
    {
        var note = _note.Generate();
        note.Mentions.Add(Mention.To(_profile.Generate()));
        var booster = _profile.Generate();
        note.BoostedBy.Add(booster);

        _timeline.HandleBoost(note);

        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<Audience>(), It.IsAny<Profile>()), Times.Never);
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<ICollection<Audience>>(), It.IsAny<Profile>()), Times.Never);
    }
    
    [Trait("TimelineService", "HandleBoost")]
    [Fact(DisplayName = "HandleBoost should add notification for creator")]
    public void AddNotificationForCreatorOnBoost()
    {
        var creator = _profile.Generate();
        var note = new FakeNote(creator).Generate();
        var booster = _profile.Generate();
        note.BoostedBy.Add(booster);

        _timeline.HandleBoost(note);

        _feeds.Verify(m => m.AddNotification(creator, note, It.IsAny<IEnumerable<Profile>>(), ActivityType.Announce), Times.Exactly(1));
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to followers timeline")]
    public void AddToFollowersOnUpdate()
    {
        var note = _note.Generate();
        var expected = Audience.FromFollowers(note.Creators.First());
        note.Visibility.Add(expected);

        _timeline.HandleUpdate(note);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Exactly(1));
    }
    
    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to all creator's followers timeline")]
    public void AddToAllFollowersOnUpdate()
    {
        var note = _note.Generate();
        note.Creators.Add(_profile.Generate());
        var audience = note.Creators.Select(Audience.FromFollowers).ToArray();
        note.Visibility.Add(Audience.Public);

        _timeline.HandleUpdate(note);

        // This assertion looks more complicated than it is.
        // It just checks that the followers audience is included for every creator on the note
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(actual =>
                    audience.Aggregate(true, (contains, expected) => contains && actual.Contains(expected))),
                It.IsAny<Profile>()), Times.Exactly(1));
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to all mentioned profiles' notifications")]
    public void AddToAllMentionedNotifications()
    {
        var note = _note.Generate();
        var mentions = _profile.Generate(3).Select(Mention.To).ToArray();
        foreach (var mention in mentions)
        {
            note.Mentions.Add(mention);
        }
        
        _timeline.HandleUpdate(note);

        foreach (var expected in mentions)
        {
            _feeds.Verify(m => m.AddNotification(expected.Subject, note, note.Creators, ActivityType.Update), Times.Once);
        }
    }
    
    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to all boosters' notifications")]
    public void AddToAllBoostersNotifications()
    {
        var note = _note.Generate();
        var boosters = _profile.Generate(3).ToArray();
        foreach (var mention in boosters)
        {
            note.BoostedBy.Add(mention);
        }
        
        _timeline.HandleUpdate(note);

        foreach (var expected in boosters)
        {
            _feeds.Verify(m => m.AddNotification(expected, note, note.Creators, ActivityType.Update), Times.Once);
        }
    }
    
    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to multiple creators' notifications")]
    public void AddToOtherCreatorsNotifications()
    {
        var note = _note.Generate();
        var localCreator = _profile.Generate();
        note.Creators.Add(localCreator);
        
        _timeline.HandleUpdate(note);
        
        _feeds.Verify(m => m.AddNotification(localCreator, note, note.Creators, ActivityType.Update), Times.Once);
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add post to mentioned profiles' feeds")]
    public void AddToMentionsOnUpdate()
    {
        var note = _note.Generate();
        var mentioned = Mention.To(_profile.Generate());
        var expected = Audience.FromMention(mentioned.Subject);
        note.Mentions.Add(mentioned);
        
        _timeline.HandleUpdate(note);

        _feeds.Verify(
            m => m.AddToTimeline(note, It.Is<ICollection<Audience>>(actual => actual.Contains(expected)),
                It.IsAny<Profile>()), Times.Exactly(1));

    }
    
    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should not add private post to any creator's followers timeline")]
    public void NoAddPrivateToFollowersOnUpdate()
    {
        var note = _note.Generate();
        note.Creators.Add(_profile.Generate());
        var audience = note.Creators.Select(Audience.FromFollowers).ToArray();
        note.Visibility.Remove(Audience.Public);

        _timeline.HandleUpdate(note);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(actual =>
                    audience.Aggregate(false, (contains, expected) => contains || actual.Contains(expected))),
                It.IsAny<Profile>()), Times.Exactly(0));
        
    }
    
    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should not add to single creator's notifications")]
    public void NoAddToSingleCreatorsNotifications()
    {
        var note = _note.Generate();
        
        _timeline.HandleUpdate(note);
        
        _feeds.Verify(m => m.AddNotification(note.Creators.First(), note, note.Creators, ActivityType.Update), Times.Never);
    }
}