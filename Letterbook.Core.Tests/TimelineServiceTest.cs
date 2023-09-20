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

    internal Note TestNote;

    public TimelineServiceTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _logger = new Mock<ILogger<TimelineService>>();
        _feeds = new Mock<IFeedsAdapter>();
        _timeline = new TimelineService(_logger.Object, CoreOptionsMock, _feeds.Object, AccountProfileMock.Object);

        _outputHelper.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _opts = CoreOptionsMock.Value;
        _note = new FakeNote();
        _profile = new FakeProfile(_opts.DomainName);
        TestNote = _note.Generate();
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
        TestNote.Visibility.Add(Audience.Public);

        _timeline.HandleCreate(TestNote);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(Audience.Public)), It.IsAny<Profile>()),
            Times.Once);
    }

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add follower posts to the creator's follower audience")]
    public void AddToFollowersOnCreate()
    {
        var expected = Audience.FromFollowers(TestNote.Creators.First());
        TestNote.Visibility.Add(expected);

        _timeline.HandleCreate(TestNote);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Once);
    }

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add public posts to the creator's follower audience")]
    public void AddToFollowersImplicitlyOnCreate()
    {
        var expected = Audience.FromFollowers(TestNote.Creators.First());
        TestNote.Visibility.Add(Audience.Public);

        _timeline.HandleCreate(TestNote);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Once);
    }

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add any posts to the mentioned profiles' feeds")]
    public void AddToMentionsOnCreate()
    {
        var mention = new Mention(_profile.Generate(), MentionVisibility.To);
        TestNote.Mentions.Add(mention);
        var expected = Audience.FromMention(mention.Subject);

        _timeline.HandleCreate(TestNote);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Once);
    }

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add any posts to the mentioned profile's notifications")]
    public void AddMentionToNotificationsOnCreate()
    {
        var profile = _profile.Generate();
        TestNote.Mentions.Add(new Mention(profile, MentionVisibility.To));

        _timeline.HandleCreate(TestNote);

        _feeds.Verify(m => m.AddNotification<Note>(profile, TestNote, TestNote.Creators, ActivityType.Create),
            Times.Once);
    }

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should add any posts to all of the mentioned profiles' notifications")]
    public void AddAllMentionsToNotificationsOnCreate()
    {
        var faker = new Faker();

        var mentions = _profile.Generate(3)
            .Select(p => new Mention(p, faker.PickRandom<MentionVisibility>())).ToArray();
        foreach (var mention in mentions)
        {
            TestNote.Mentions.Add(mention);
        }

        _timeline.HandleCreate(TestNote);

        foreach (var mention in mentions)
        {
            _feeds.Verify(m => m.AddNotification(mention.Subject, TestNote, TestNote.Creators, ActivityType.Create),
                Times.Once);
        }
    }

    [Trait("TimelineService", "HandleCreate")]
    [Fact(DisplayName = "HandleCreate should not add private posts to the public or follower feeds")]
    public void NoAddPrivateOnCreate()
    {
        TestNote.Visibility.Remove(Audience.Public);
        TestNote.Visibility.Remove(Audience.FromFollowers(TestNote.Creators.First()));
        var mentioned = Mention.To(_profile.Generate());
        TestNote.Mentions.Add(mentioned);

        _timeline.HandleCreate(TestNote);

        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), Audience.Public, It.IsAny<Profile>()), Times.Never);
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(), Audience.FromFollowers(TestNote.Creators.First()),
                It.IsAny<Profile>()),
            Times.Never);
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(Audience.Public)), It.IsAny<Profile>()),
            Times.Never);
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience =>
                    audience.Contains(Audience.FromFollowers(TestNote.Creators.First()))), It.IsAny<Profile>()),
            Times.Never);
    }

    [Trait("TimelineService", "HandleBoost")]
    [Fact(DisplayName = "HandleBoost should add public posts to the boost feed")]
    public void AddPublicToTimelineOnBoost()
    {
        TestNote.Visibility.Add(Audience.Public);
        var booster = _profile.Generate();
        TestNote.BoostedBy.Add(booster);

        _timeline.HandleBoost(TestNote);

        _feeds.Verify(m => m.AddToTimeline(TestNote, Audience.FromBoost(booster), booster), Times.Once);
    }

    [Trait("TimelineService", "HandleBoost")]
    [Fact(DisplayName = "HandleBoost should not add follower posts to any feed")]
    public void NoAddFollowersToTimelineOnBoost()
    {
        TestNote.Visibility.Add(Audience.FromFollowers(TestNote.Creators.First()));
        var booster = _profile.Generate();
        TestNote.BoostedBy.Add(booster);

        _timeline.HandleBoost(TestNote);

        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<Audience>(), It.IsAny<Profile>()), Times.Never);
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<ICollection<Audience>>(), It.IsAny<Profile>()),
            Times.Never);
    }

    [Trait("TimelineService", "HandleBoost")]
    [Fact(DisplayName = "HandleBoost should not add private posts to any feed")]
    public void NoAddPrivateToTimelineOnBoost()
    {
        TestNote.Mentions.Add(Mention.To(_profile.Generate()));
        var booster = _profile.Generate();
        TestNote.BoostedBy.Add(booster);

        _timeline.HandleBoost(TestNote);

        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<Audience>(), It.IsAny<Profile>()), Times.Never);
        _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<ICollection<Audience>>(), It.IsAny<Profile>()),
            Times.Never);
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

        _feeds.Verify(m => m.AddNotification(creator, note, It.IsAny<IEnumerable<Profile>>(), ActivityType.Announce),
            Times.Once);
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to followers timeline")]
    public void AddToFollowersOnUpdate()
    {
        var expected = Audience.FromFollowers(TestNote.Creators.First());
        TestNote.Visibility.Add(expected);

        _timeline.HandleUpdate(TestNote);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Once);
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to all creator's followers timeline")]
    public void AddToAllFollowersOnUpdate()
    {
        TestNote.Creators.Add(_profile.Generate());
        var audience = TestNote.Creators.Select(Audience.FromFollowers).ToArray();
        TestNote.Visibility.Add(Audience.Public);

        _timeline.HandleUpdate(TestNote);

        // This assertion looks more complicated than it is.
        // It just checks that the followers audience is included for every creator on the note
        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(actual =>
                    audience.Aggregate(true, (contains, expected) => contains && actual.Contains(expected))),
                It.IsAny<Profile>()), Times.Once);
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to all mentioned profiles' notifications")]
    public void AddToAllMentionedNotifications()
    {
        var mentions = _profile.Generate(3).Select(Mention.To).ToArray();
        foreach (var mention in mentions)
        {
            TestNote.Mentions.Add(mention);
        }

        _timeline.HandleUpdate(TestNote);

        foreach (var expected in mentions)
        {
            _feeds.Verify(m => m.AddNotification(expected.Subject, TestNote, TestNote.Creators, ActivityType.Update),
                Times.Once);
        }
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to all boosters' notifications")]
    public void AddToAllBoostersNotifications()
    {
        var boosters = _profile.Generate(3).ToArray();
        foreach (var mention in boosters)
        {
            TestNote.BoostedBy.Add(mention);
        }

        _timeline.HandleUpdate(TestNote);

        foreach (var expected in boosters)
        {
            _feeds.Verify(m => m.AddNotification(expected, TestNote, TestNote.Creators, ActivityType.Update),
                Times.Once);
        }
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add to multiple creators' notifications")]
    public void AddToOtherCreatorsNotifications()
    {
        var localCreator = _profile.Generate();
        TestNote.Creators.Add(localCreator);

        _timeline.HandleUpdate(TestNote);

        _feeds.Verify(m => m.AddNotification(localCreator, TestNote, TestNote.Creators, ActivityType.Update),
            Times.Once);
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should add post to mentioned profiles' feeds")]
    public void AddToMentionsOnUpdate()
    {
        var mentioned = Mention.To(_profile.Generate());
        var expected = Audience.FromMention(mentioned.Subject);
        TestNote.Mentions.Add(mentioned);

        _timeline.HandleUpdate(TestNote);

        _feeds.Verify(
            m => m.AddToTimeline(TestNote, It.Is<ICollection<Audience>>(actual => actual.Contains(expected)),
                It.IsAny<Profile>()), Times.Once);
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should not add private post to any creator's followers timeline")]
    public void NoAddPrivateToFollowersOnUpdate()
    {
        TestNote.Creators.Add(_profile.Generate());
        var audience = TestNote.Creators.Select(Audience.FromFollowers).ToArray();
        TestNote.Visibility.Remove(Audience.Public);

        _timeline.HandleUpdate(TestNote);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Note>(),
                It.Is<ICollection<Audience>>(actual =>
                    audience.Aggregate(false, (contains, expected) => contains || actual.Contains(expected))),
                It.IsAny<Profile>()), Times.Never);
    }

    [Trait("TimelineService", "HandleUpdate")]
    [Fact(DisplayName = "HandleUpdate should not add to single creator's notifications")]
    public void NoAddToSingleCreatorsNotifications()
    {
        _timeline.HandleUpdate(TestNote);

        _feeds.Verify(
            m => m.AddNotification(TestNote.Creators.First(), TestNote, TestNote.Creators, ActivityType.Update),
            Times.Never);
    }

    [Trait("TimelineService", "HandleDelete")]
    [Fact(DisplayName = "HandleDelete should remove the deleted post from all feeds")]
    public void RemoveFromFeedsOnDelete()
    {
        _timeline.HandleDelete(TestNote);

        _feeds.Verify(m => m.RemoveFromTimelines(TestNote), Times.Once);
    }
}