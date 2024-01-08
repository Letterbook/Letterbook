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

    internal Post TestPost;

    public TimelineServiceTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _logger = new Mock<ILogger<TimelineService>>();
        _feeds = new Mock<IFeedsAdapter>();
        _timeline = new TimelineService(_logger.Object, CoreOptionsMock, _feeds.Object, AccountProfileMock.Object);

        _outputHelper.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _opts = CoreOptionsMock.Value;
        _profile = new FakeProfile(_opts.DomainName);
        _note = new FakeNote(_profile);
        TestPost = _note.Generate();
    }

    [Fact(Skip = "Broken")]
    public void Exists()
    {
        Assert.NotNull(_timeline);
    }

    
    [Fact(DisplayName = "HandleCreate should add public posts to the public audience", Skip = "Broken")]
    public void AddToPublicOnCreate()
    {
        TestPost.Audience.Add(Audience.Public);

        _timeline.HandleCreate(TestPost);

        _feeds.Verify(
            m => m.AddToTimeline(It.IsAny<Post>(), It.IsAny<Profile>()),
            Times.Once);
    }

    
    [Fact(DisplayName = "HandleCreate should add follower posts to the creator's follower audience", Skip = "Broken")]
    public void AddToFollowersOnCreate()
    {
        var expected = Audience.Followers(TestPost.Creators.First());
        TestPost.Audience.Add(expected);

        _timeline.HandleCreate(TestPost);

        _feeds.Verify(
            m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Once);
    }

    
    [Fact(DisplayName = "HandleCreate should add public posts to the creator's follower audience", Skip = "Broken")]
    public void AddToFollowersImplicitlyOnCreate()
    {
        var expected = Audience.Followers(TestPost.Creators.First());
        TestPost.Audience.Add(Audience.Public);

        _timeline.HandleCreate(TestPost);

        _feeds.Verify(
            m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Once);
    }

    
    [Fact(DisplayName = "HandleCreate should add any posts to the mentioned profiles' feeds", Skip = "Broken")]
    public void AddToMentionsOnCreate()
    {
        var mention = new Mention(_profile.Generate(), MentionVisibility.To);
        TestPost.AddressedTo.Add(mention);
        var expected = Audience.FromMention(mention.Subject);

        _timeline.HandleCreate(TestPost);

        _feeds.Verify(
            m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()),
            Times.Once);
    }

    
    [Fact(DisplayName = "HandleCreate should add any posts to the mentioned profile's notifications", Skip = "Broken")]
    public void AddMentionToNotificationsOnCreate()
    {
        var profile = _profile.Generate();
        TestPost.AddressedTo.Add(new Mention(profile, MentionVisibility.To));

        _timeline.HandleCreate(TestPost);

        _feeds.Verify(m => m.AddNotification(profile, TestPost, ActivityType.Create, It.IsAny<Profile?>()),
            Times.Once);
    }

    
    [Fact(DisplayName = "HandleCreate should add any posts to all of the mentioned profiles' notifications", Skip = "Broken")]
    public void AddAllMentionsToNotificationsOnCreate()
    {
        var faker = new Faker();

        var mentions = _profile.Generate(3)
            .Select(p => new Mention(p, faker.PickRandom<MentionVisibility>())).ToArray();
        foreach (var mention in mentions)
        {
            TestPost.AddressedTo.Add(mention);
        }

        _timeline.HandleCreate(TestPost);

        foreach (var mention in mentions)
        {
            _feeds.Verify(m => m.AddNotification(mention.Subject, TestPost, ActivityType.Create, It.IsAny<Profile?>()),
                Times.Once);
        }
    }

    
    [Fact(DisplayName = "HandleCreate should not add private posts to the public or follower feeds",
        Skip = "Broken")]
    public void NoAddPrivateOnCreate()
    {
        TestPost.Audience.Remove(Audience.Public);
        TestPost.Audience.Remove(Audience.Followers(TestPost.Creators.First()));
        var mentioned = Mention.To(_profile.Generate());
        TestPost.AddressedTo.Add(mentioned);

        _timeline.HandleCreate(TestPost);

        // _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), Audience.Public, It.IsAny<Profile>()), Times.Never);
        // _feeds.Verify(
        //     m => m.AddToTimeline(It.IsAny<Note>(), Audience.Followers(TestPost.Creators.First()),
        //         It.IsAny<Profile>()),
        //     Times.Never);
        // _feeds.Verify(
        //     m => m.AddToTimeline(It.IsAny<Note>(),
        //         It.Is<ICollection<Audience>>(audience => audience.Contains(Audience.Public)), It.IsAny<Profile>()),
        //     Times.Never);
        // _feeds.Verify(
        //     m => m.AddToTimeline(It.IsAny<Note>(),
        //         It.Is<ICollection<Audience>>(audience =>
        //             audience.Contains(Audience.Followers(TestPost.Creators.First()))), It.IsAny<Profile>()),
        //     Times.Never);
    }

    
    [Fact(DisplayName = "HandleBoost should add public posts to the boost feed",
        Skip = "Broken")]
    public void AddPublicToTimelineOnBoost()
    {
        TestPost.Audience.Add(Audience.Public);
        var booster = _profile.Generate();
        TestPost.SharesCollection.Add(booster);

        _timeline.HandleBoost(TestPost);

        // _feeds.Verify(m => m.AddToTimeline(TestPost, Audience.Boosts(booster), booster), Times.Once);
    }

    
    [Fact(DisplayName = "HandleBoost should not add follower posts to any feed",
        Skip = "Broken")]
    public void NoAddFollowersToTimelineOnBoost()
    {
        TestPost.Audience.Add(Audience.Followers(TestPost.Creators.First()));
        var booster = _profile.Generate();
        TestPost.SharesCollection.Add(booster);

        _timeline.HandleBoost(TestPost);

        // _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<Audience>(), It.IsAny<Profile>()), Times.Never);
        // _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<ICollection<Audience>>(), It.IsAny<Profile>()),
            // Times.Never);
    }

    
    [Fact(DisplayName = "HandleBoost should not add private posts to any feed", Skip = "Broken")]
    public void NoAddPrivateToTimelineOnBoost()
    {
        TestPost.AddressedTo.Add(Mention.To(_profile.Generate()));
        var booster = _profile.Generate();
        TestPost.SharesCollection.Add(booster);

        _timeline.HandleBoost(TestPost);

        // _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<Audience>(), It.IsAny<Profile>()), Times.Never);
        // _feeds.Verify(m => m.AddToTimeline(It.IsAny<Note>(), It.IsAny<ICollection<Audience>>(), It.IsAny<Profile>()),
            // Times.Never);
    }

    
    [Fact(DisplayName = "HandleBoost should add notification for creator", Skip = "Broken")]
    public void AddNotificationForCreatorOnBoost()
    {
        var creator = _profile.Generate();
        // var note = new FakeNote(creator).Generate();
        var booster = _profile.Generate();
        // note.SharesCollection.Add(booster);

        // _timeline.HandleBoost(note);

        // _feeds.Verify(m => m.AddNotification(creator, note, It.IsAny<IEnumerable<Profile>>(), ActivityType.Announce),
            // Times.Once);
    }

    
    [Fact(DisplayName = "HandleUpdate should add to followers timeline", Skip = "Broken")]
    public void AddToFollowersOnUpdate()
    {
        var expected = Audience.Followers(TestPost.Creators.First());
        TestPost.Audience.Add(expected);

        _timeline.HandleUpdate(TestPost);

        // _feeds.Verify(
            // m => m.AddToTimeline(It.IsAny<Note>(),
                // It.Is<ICollection<Audience>>(audience => audience.Contains(expected)), It.IsAny<Profile>()),
            // Times.Once);
    }

    
    [Fact(DisplayName = "HandleUpdate should add to all creator's followers timeline", Skip = "Broken")]
    public void AddToAllFollowersOnUpdate()
    {
        TestPost.Creators.Add(_profile.Generate());
        var audience = TestPost.Creators.Select(Audience.Followers).ToArray();
        TestPost.Audience.Add(Audience.Public);

        _timeline.HandleUpdate(TestPost);

        // This assertion looks more complicated than it is.
        // It just checks that the followers audience is included for every creator on the note
        // _feeds.Verify(
            // m => m.AddToTimeline(It.IsAny<Note>(),
                // It.Is<ICollection<Audience>>(actual =>
                    // audience.Aggregate(true, (contains, expected) => contains && actual.Contains(expected))),
                // It.IsAny<Profile>()), Times.Once);
    }

    
    [Fact(DisplayName = "HandleUpdate should add to all mentioned profiles' notifications", Skip = "Broken")]
    public void AddToAllMentionedNotifications()
    {
        var mentions = _profile.Generate(3).Select(Mention.To).ToArray();
        foreach (var mention in mentions)
        {
            TestPost.AddressedTo.Add(mention);
        }

        _timeline.HandleUpdate(TestPost);

        foreach (var expected in mentions)
        {
            // _feeds.Verify(m => m.AddNotification(expected.Subject, TestPost, TestPost.Creators, ActivityType.Update),
                // Times.Once);
        }
    }

    
    [Fact(DisplayName = "HandleUpdate should add to all boosters' notifications",Skip = "Broken")]
    public void AddToAllBoostersNotifications()
    {
        var boosters = _profile.Generate(3).ToArray();
        foreach (var mention in boosters)
        {
            // TestPost.AddressedTo.Add(mention);
        }

        _timeline.HandleUpdate(TestPost);

        foreach (var expected in boosters)
        {
            // _feeds.Verify(m => m.AddNotification(expected, TestPost, TestPost.Creators, ActivityType.Update),
                // Times.Once);
        }
    }

    
    [Fact(DisplayName = "HandleUpdate should add to multiple creators' notifications",Skip = "Broken")]
    public void AddToOtherCreatorsNotifications()
    {
        var localCreator = _profile.Generate();
        TestPost.Creators.Add(localCreator);

        _timeline.HandleUpdate(TestPost);

        // _feeds.Verify(m => m.AddNotification(localCreator, TestPost, TestPost.Creators, ActivityType.Update),
            // Times.Once);
    }

    
    [Fact(DisplayName = "HandleUpdate should add post to mentioned profiles' feeds",Skip = "Broken")]
    public void AddToMentionsOnUpdate()
    {
        var mentioned = Mention.To(_profile.Generate());
        var expected = Audience.FromMention(mentioned.Subject);
        TestPost.AddressedTo.Add(mentioned);

        _timeline.HandleUpdate(TestPost);

        // _feeds.Verify(
            // m => m.AddToTimeline(TestPost, It.Is<ICollection<Audience>>(actual => actual.Contains(expected)),
                // It.IsAny<Profile>()), Times.Once);
    }

    
    [Fact(DisplayName = "HandleUpdate should not add private post to any creator's followers timeline", Skip = "Broken")]
    public void NoAddPrivateToFollowersOnUpdate()
    {
        TestPost.Creators.Add(_profile.Generate());
        var audience = TestPost.Creators.Select(Audience.Followers).ToArray();
        TestPost.Audience.Remove(Audience.Public);

        _timeline.HandleUpdate(TestPost);

        // _feeds.Verify(
            // m => m.AddToTimeline(It.IsAny<Note>(),
                // It.Is<ICollection<Audience>>(actual =>
                    // audience.Aggregate(false, (contains, expected) => contains || actual.Contains(expected))),
                // It.IsAny<Profile>()), Times.Never);
    }

    
    [Fact(DisplayName = "HandleUpdate should not add to single creator's notifications", Skip = "Broken")]
    public void NoAddToSingleCreatorsNotifications()
    {
        _timeline.HandleUpdate(TestPost);

        // _feeds.Verify(
            // m => m.AddNotification(TestPost.Creators.First(), TestPost, TestPost.Creators, ActivityType.Update),
            // Times.Never);
    }

    
    [Fact(DisplayName = "HandleDelete should remove the deleted post from all feeds", Skip = "Broken")]
    public void RemoveFromFeedsOnDelete()
    {
        _timeline.HandleDelete(TestPost);

        _feeds.Verify(m => m.RemoveFromTimelines(TestPost), Times.Once);
    }
}

public class FakeNote : Faker<Post>
{
    public FakeNote(FakeProfile profile)
    {
        throw new NotImplementedException();
    }
}