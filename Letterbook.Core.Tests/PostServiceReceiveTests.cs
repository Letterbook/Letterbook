using System.Net.Mime;
using System.Security.Claims;
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

	[Trait("ActivityPub", "Create")]
	[Fact(DisplayName = "Should create new received posts")]
	public async Task ShouldReceiveCreate()
	{
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());

		var actual = await _service.ReceiveCreate([_post]);

		Assert.Single(actual);
	}

	[Trait("ActivityPub", "Create")]
	[Fact(DisplayName = "Should create multiple received posts")]
	public async Task ShouldReceiveCreate_Multiple()
	{
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.BuildMock());
		var reply = new FakePost(_actor, _post).Generate();

		var actual = await _service.ReceiveCreate([_post, reply]);

		Assert.Equal(2, actual.Count());
	}

	[Trait("ActivityPub", "Create")]
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

	[Trait("ActivityPub", "Create")]
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

	[Trait("ActivityPub", "Create")]
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

	[Trait("ActivityPub", "Create")]
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

	[Trait("ActivityPub", "Create")]
	[Fact(DisplayName = "Should crawl posts instead of create from unknown actors")]
	public async Task CreateShouldCrawlUnknownActors()
	{
		var actual = await _service.ReceiveCreate([_post]);

		Assert.Empty(actual);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlProfile(It.IsAny<ProfileId>(), _actor.FediId, 0));
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), _post.FediId, 0));
	}

	[Trait("ActivityPub", "Create")]
	[Fact(DisplayName = "Should crawl posts instead of create from invalid actors")]
	public async Task CreateShouldCrawlInvalidActors()
	{
		_service.As([]);
		var actual = await _service.ReceiveCreate([_post]);

		Assert.Empty(actual);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlPost(It.IsAny<ProfileId>(), _post.FediId, 1));
	}

	[Trait("ActivityPub", "Update")]
	[Fact(DisplayName = "Should update posts")]
	public async Task ShouldReceiveUpdate()
	{
		var updated = _post.ShallowClone();
		updated.Contents = new List<Content>()
		{
			new Note
			{
				FediId = _post.Contents.First().FediId,
				Post = updated,
				SortKey = 0,
				ContentType = new ContentType("text/html"),
				Html = "<p>this is the updated text</p>",
			}
		};

		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile> { _actor }.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.Append(Audience.Public).BuildMock());
		DataAdapterMock.Setup(m => m.ListPosts(It.IsAny<IEnumerable<Uri>>())).Returns(new List<Post> { _post }.BuildMock());

		var actual = await _service.ReceiveUpdate([updated]);

		Assert.Single(actual);
	}

	[Trait("ActivityPub", "Update")]
	[Fact(DisplayName = "Should create unknown post when received as update")]
	public async Task ShouldReceiveUpdate_Unknown()
	{
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.Append(Audience.Public).BuildMock());

		var actual = await _service.ReceiveUpdate([_post]);

		Assert.Single(actual);
	}

	[Trait("ActivityPub", "Update")]
	[Fact(DisplayName = "Should update AddressedTo")]
	public async Task ShouldReceiveUpdate_Addressee()
	{
		var updated = _post.ShallowClone();
		var addressedTo = _fakeActor.Generate();
		updated.AddressedTo.Add(new(addressedTo, MentionVisibility.To));

		DataAdapterMock.Setup(m => m.ListPosts(It.IsAny<IEnumerable<Uri>>())).Returns(new List<Post> { _post }.BuildMock());
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.ListProfiles(It.IsAny<IEnumerable<Uri>>()))
			.Returns(new List<Profile>() { _actor, addressedTo }.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.Append(Audience.Public).BuildMock());

		var actual = await _service.ReceiveUpdate([_post]);

		Assert.Single(actual);
	}

	[Trait("ActivityPub", "Update")]
	[Fact(DisplayName = "Should crawl unknown mention on update")]
	public async Task ShouldReceiveUpdate_AddresseeUnknown()
	{
		var updated = _post.ShallowClone();
		var addressedTo = _fakeActor.Generate();
		updated.AddressedTo.Add(new(addressedTo, MentionVisibility.To));

		DataAdapterMock.Setup(m => m.ListPosts(It.IsAny<IEnumerable<Uri>>())).Returns(new List<Post> { _post }.BuildMock());
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile>{_actor}.BuildMock());
		DataAdapterMock.Setup(m => m.ListProfiles(It.IsAny<IEnumerable<Uri>>()))
			.Returns(new List<Profile>() { _actor }.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.Append(Audience.Public).BuildMock());

		var actual = await _service.ReceiveUpdate([_post]);

		Assert.Single(actual);
		ApCrawlerSchedulerMock.Verify(m => m.CrawlProfile(It.IsAny<ProfileId>(), addressedTo.FediId, It.IsAny<int>()));
	}

	[Trait("ActivityPub", "Delete")]
	[Fact(DisplayName = "Should delete posts")]
	public async Task ShouldReceiveDelete()
	{
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile> { _actor }.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.Append(Audience.Public).BuildMock());
		DataAdapterMock.Setup(m => m.ListPosts(It.IsAny<IEnumerable<Uri>>())).Returns(new List<Post> { _post }.BuildMock());

		var actual = await _service.ReceiveDelete([_post.FediId]);

		Assert.Single(actual);
	}

	[Trait("ActivityPub", "Delete")]
	[Fact(DisplayName = "Should delete posts")]
	public async Task ShouldReceiveDelete_IgnoreUnknown()
	{
		DataAdapterMock.Setup(m => m.SingleProfile(_actor.FediId)).Returns(new List<Profile> { _actor }.BuildMock());
		DataAdapterMock.Setup(m => m.QueryAudience()).Returns(_actor.Headlining.Append(Audience.Public).BuildMock());
		DataAdapterMock.Setup(m => m.ListPosts(It.IsAny<IEnumerable<Uri>>())).Returns(new List<Post> { }.BuildMock());

		var actual = await _service.ReceiveDelete([_post.FediId]);

		Assert.Empty(actual);
	}
}