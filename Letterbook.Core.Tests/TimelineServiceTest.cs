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
	private readonly Profile _profile;
	private TimelineService _timeline;
	private readonly Post _testPost;

	public TimelineServiceTest(ITestOutputHelper outputHelper)
	{
		_outputHelper = outputHelper;
		_logger = new Mock<ILogger<TimelineService>>();
		_feeds = new Mock<IFeedsAdapter>();
		_timeline = new TimelineService(_logger.Object, CoreOptionsMock, _feeds.Object, AccountProfileMock.Object);

		_outputHelper.WriteLine($"Bogus Seed: {Init.WithSeed()}");
		_opts = CoreOptionsMock.Value;
		_profile = new FakeProfile(_opts.DomainName).Generate();
		_testPost = new FakePost(_profile).Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_timeline);
	}


	[Fact(DisplayName = "HandlePublish should add public posts to the public audience")]
	public async Task AddToPublicOnCreate()
	{
		_testPost.Audience.Add(Audience.Public);

		await _timeline.HandlePublish(_testPost);

		_feeds.Verify(
			m => m.AddToTimeline(It.IsAny<Post>(), It.IsAny<Profile>()),
			Times.Once);
	}


	[Fact(DisplayName = "HandlePublish should add follower posts to the creator's follower audience")]
	public async Task AddToFollowersOnCreate()
	{
		var expected = Audience.Followers(_testPost.Creators.First());
		_testPost.Audience.Add(expected);

		await _timeline.HandlePublish(_testPost);

		_feeds.Verify(
			m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()),
			Times.Once);
	}


	[Fact(DisplayName = "HandlePublish should add public posts to the creator's follower audience")]
	public async Task AddToFollowersImplicitlyOnCreate()
	{
		var expected = Audience.Followers(_testPost.Creators.First());
		_testPost.Audience.Add(Audience.Public);

		await _timeline.HandlePublish(_testPost);

		_feeds.Verify(
			m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()),
			Times.Once);
	}


	[Fact(DisplayName = "HandlePublish should add posts to feed for anyone mentioned in the post")]
	public async Task AddToMentionsOnCreate()
	{
		var mentioned = new FakeProfile("letterbook.example").Generate();
		var mention = new Mention(mentioned, MentionVisibility.To);
		_testPost.AddressedTo.Add(mention);
		var expected = Audience.FromMention(mention.Subject);

		await _timeline.HandlePublish(_testPost);

		_feeds.Verify(
			m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()),
			Times.Once);
	}

	[Fact(DisplayName = "HandlePublish should not add private posts to the public or follower feeds")]
	public async Task NoAddPrivateOnCreate()
	{
		var mentioned = new FakeProfile("letterbook.example").Generate();
		_testPost.Audience.Clear();
		_testPost.Audience.Remove(Audience.Followers(_testPost.Creators.First()));
		var mention = Mention.To(mentioned);
		_testPost.AddressedTo.Add(mention);

		await _timeline.HandlePublish(_testPost);

		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(Audience.Public)), It.IsAny<Profile>()), Times.Never);
		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(Audience.Followers(_profile))), It.IsAny<Profile>()),
			Times.Never);
	}


	[Fact(DisplayName = "HandleShare should add public posts to the boost feed")]
	public async Task AddPublicToTimelineOnBoost()
	{
		_testPost.Audience.Add(Audience.Public);
		var booster = _profile;
		_testPost.SharesCollection.Add(booster);

		await _timeline.HandleShare(_testPost, _profile);

		_feeds.Verify(m => m.AddToTimeline(_testPost, booster), Times.Once);
	}

	[Fact(DisplayName = "HandleShare should not add follower-only posts to public feeds")]
	public async Task NoAddFollowersToTimelineOnBoost()
	{
		_testPost.Audience.Clear();
		_testPost.Audience.Add(Audience.Followers(_testPost.Creators.First()));
		var booster = _profile;
		_testPost.SharesCollection.Add(booster);

		await _timeline.HandleShare(_testPost, booster);

		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(Audience.Public)), It.IsAny<Profile>()), Times.Never);
	}

	[Fact(DisplayName = "HandleShare should not add private posts to public feeds")]
	public async Task NoAddPrivateToTimelineOnBoost()
	{
		_testPost.AddressedTo.Clear();
		_testPost.Audience.Clear();
		_testPost.AddressedTo.Add(Mention.To(_profile));
		var booster = _profile;
		_testPost.SharesCollection.Add(booster);

		await _timeline.HandleShare(_testPost, booster);

		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(Audience.Public)), It.IsAny<Profile>()), Times.Never);
		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(Audience.Followers(_profile))), It.IsAny<Profile>()),
			Times.Never);
	}

	[Fact(DisplayName = "HandleUpdate should add to followers timeline")]
	public async Task AddToFollowersOnUpdate()
	{
		var expected = Audience.Followers(_testPost.Creators.First());
		_testPost.Audience.Add(expected);

		await _timeline.HandleUpdate(_testPost, TODO);

		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()), Times.Once);
	}


	[Fact(DisplayName = "HandleUpdate should add to all creator's followers timeline")]
	public async Task AddToAllFollowersOnUpdate()
	{
		_testPost.Creators.Add(_profile);
		var audience = _testPost.Creators.Select(Audience.Followers).ToArray();
		_testPost.Audience.Add(Audience.Public);

		await _timeline.HandleUpdate(_testPost, TODO);

		// This assertion looks more complicated than it is.
		// It just checks that the followers audience is included for every creator on the note
		_feeds.Verify(
			m => m.AddToTimeline(
				It.Is<Post>(p => p.Audience.Aggregate(true, (contains, expected) => contains && p.Audience.Contains(expected))),
				It.IsAny<Profile>()), Times.Once);
	}

	[Fact(DisplayName = "HandleUpdate should add post to mentioned profiles' feeds")]
	public async Task AddToMentionsOnUpdate()
	{
		var oldPost = _testPost.ShallowClone();
		var mentioned = Mention.To(_profile);
		var expected = Audience.FromMention(mentioned.Subject);
		_testPost.AddressedTo.Add(mentioned);

		await _timeline.HandleUpdate(_testPost, oldPost);

		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(expected)), It.IsAny<Profile>()), Times.Once);
	}


	[Fact(DisplayName = "HandleUpdate should not add private post to any creator's followers timeline")]
	public async Task NoAddPrivateToFollowersOnUpdate()
	{
		_testPost.Creators.Add(_profile);
		_testPost.Audience.Clear();

		await _timeline.HandleUpdate(_testPost, TODO);

		_feeds.Verify(m => m.AddToTimeline(It.Is<Post>(p => p.Audience.Contains(Audience.Public)), It.IsAny<Profile>()), Times.Never);
	}

	[Fact(DisplayName = "HandleDelete should remove the deleted post from all feeds")]
	public async Task RemoveFromFeedsOnDelete()
	{
		await _timeline.HandleDelete(_testPost);

		_feeds.Verify(m => m.RemoveFromTimelines(_testPost), Times.Once);
	}
}