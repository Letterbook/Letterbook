using System.Security.Claims;
using AngleSharp.Common;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Mocks;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class PostServiceReceiveTests : WithMocks
{
	private readonly ITestOutputHelper _output;
	private readonly PostService _service;
	private readonly FakeProfile _fakeActor;
	private readonly Profile _actor;
	private readonly Post _post;

	public PostServiceReceiveTests(ITestOutputHelper output)
	{
		_output = output;
		_output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_service = new PostService(Mock.Of<ILogger<PostService>>(), CoreOptionsMock, Mock.Of<Instrumentation>(),
			DataAdapterMock.Object, PostEventServiceMock.Object, ProfileEventServiceMock.Object,
			ApCrawlerSchedulerMock.Object, [new MockHtmlSanitizer(), new MockTextSanitizer()]);
		_fakeActor = new FakeProfile("peer.example");
		_actor = _fakeActor.Generate();
		_post = new FakePost(_actor).Generate();

		_service.As([new Claim(ApplicationClaims.Actor, _actor.FediId.ToString())]);

		DataAdapterMock.Setup(m => m.ListPosts(It.IsAny<IEnumerable<Uri>>())).Returns(Array.Empty<Post>().BuildMock());
		DataAdapterMock.Setup(m => m.ListProfiles(It.IsAny<IEnumerable<Uri>>())).Returns(Array.Empty<Profile>().BuildMock());
		DataAdapterMock.Setup(m => m.Threads(It.IsAny<Uri[]>())).Returns(Array.Empty<ThreadContext>().BuildMock());
		DataAdapterMock.Setup(m => m.SingleProfile(It.IsAny<Uri>())).Returns(Array.Empty<Profile>().BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(Array.Empty<Audience>().BuildMock());
	}

	public class UriComparer : IEqualityComparer<Uri>
	{
		public bool Equals(Uri? x, Uri? y) => x?.ToString() == y?.ToString();

		public int GetHashCode(Uri obj) => obj.ToString().GetHashCode();
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should create new received posts")]
	public async Task ShouldReceiveCreate()
	{
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());

		var actual = await _service.ReceiveCreate([_post]);

		Assert.Single(actual);
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should create multiple received posts")]
	public async Task ShouldReceiveCreate_Multiple()
	{
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());
		var reply = new FakePost(_actor, _post).Generate();

		var actual = await _service.ReceiveCreate([_post, reply]);

		Assert.Equal(2, actual.Count());
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should create new received posts in reply to known posts")]
	public async Task ShouldReceiveCreate_Reply()
	{
		var parent = new FakePost("peer.example").Generate();
		_post.InReplyTo = parent;

		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.ListPosts(It.IsAny<IEnumerable<Uri>>())).Returns(new []{parent}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());

		var actual = await _service.ReceiveCreate([_post]);

		Assert.Single(actual);
		DataAdapterMock.Verify(m => m.ListPosts(It.Is<IEnumerable<Uri>>(l => l.Contains(parent.FediId))), Times.Once);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), parent.FediId, 0), Times.Never);
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should create new received posts that mention known profiles")]
	public async Task ShouldReceiveCreate_Mention()
	{
		var peer = new FakeProfile("letterbook.example").Generate();
		_post.AddressedTo.Add(new Mention(peer, MentionVisibility.To));

		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.ListProfiles(It.IsAny<IEnumerable<Uri>>())).Returns(new List<Profile>{_actor, peer}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());

		var actual = await _service.ReceiveCreate([_post]);

		Assert.Single(actual);
		Assert.Contains(new Mention(peer, MentionVisibility.To), actual.First().AddressedTo);
		DataAdapterMock.Verify(m => m.ListProfiles(It.Is<IEnumerable<Uri>>(l => l.Contains(peer.FediId))), Times.Once);
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should crawl unknown Profiles from received create")]
	public async Task ShouldReceiveCreate_MentionUnknown()
	{
		var peer = new FakeProfile("letterbook.example").Generate();
		_post.AddressedTo.Add(new Mention(peer, MentionVisibility.To));

		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.ListProfiles(It.IsAny<IEnumerable<Uri>>())).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());

		var actual = await _service.ReceiveCreate([_post]);

		Assert.Single(actual);
		Assert.Contains(new Mention(peer, MentionVisibility.To), actual.First().AddressedTo);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlProfile(It.IsAny<ProfileId>(), peer.FediId, 0));
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should crawl unknown Posts from received create")]
	public async Task ShouldReceiveCreate_ReferenceUnknown()
	{
		var parent = new Post(new Uri("https://peer.example/post/this-is-a-test"), new ThreadContext(default, new CoreOptions()));
		_post.InReplyTo = parent;

		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());

		var actual = await _service.ReceiveCreate([_post]);

		Assert.Equal(2, actual.Count());
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), parent.FediId, 0));
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should crawl posts instead of create from unknown actors")]
	public async Task CreateShouldCrawlUnknownActors()
	{
		var actual = await _service.ReceiveCreate([_post]);

		Assert.Empty(actual);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlProfile(It.IsAny<ProfileId>(), _actor.FediId, 0));
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), _post.FediId, 0));
	}

	[Trait("Activity", "Create")]
	[Fact(DisplayName = "Should crawl posts instead of create from invalid actors")]
	public async Task CreateShouldCrawlInvalidActors()
	{
		_service.As([]);
		var actual = await _service.ReceiveCreate([_post]);

		Assert.Empty(actual);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), _post.FediId, 1));
	}
}