using Letterbook.Adapter.Db;
using Letterbook.Core.Models;
using Letterbook.IntegrationTests.Fixtures;
using Medo;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Adapter")]
public class PostAdapterTests : IClassFixture<HostFixture<PostAdapterTests>>, ITestSeed
{
	private readonly ITestOutputHelper _output;
	private readonly HostFixture<PostAdapterTests> _host;
	private PostAdapter _adapter;
	private RelationalContext _context;
	private RelationalContext _actual;
	private Dictionary<Profile, List<Post>> _posts;
	private List<Profile> _profiles;
	private static ValueComparer<Post> _cmp;
	static int? ITestSeed.Seed() => null;

	public PostAdapterTests(ITestOutputHelper output, HostFixture<PostAdapterTests> host)
	{
		_output = output;
		_host = host;
		_posts = host.Posts;
		_profiles = host.Profiles;

		_context = host.CreateContext();
		_actual = host.CreateContext();
		_adapter = new PostAdapter(Mock.Of<ILogger<PostAdapter>>(), _context);
	}

	static PostAdapterTests()
	{
		_cmp = new ValueComparer<Post>(
			(l, r) => r != null && l != null && l.FediId == r.FediId,
			p => p.FediId.GetHashCode());
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_adapter);
	}

	[Fact(DisplayName = "Should lookup posts by ID")]
	public async Task CanLookupById()
	{
		var expected = _posts[_profiles[0]][0];
		var actual = await _adapter.LookupPost(expected.GetId());

		Assert.True(expected.GetId() == actual?.GetId());
		Assert.Equal(expected, actual, _cmp);
	}

	[Fact(DisplayName = "Should lookup posts by FediID")]
	public async Task CanLookupByFediId()
	{
		var expected = _posts[_profiles[0]][0];
		var actual = await _adapter.LookupPost(expected.FediId);

		Assert.True(expected.GetId() == actual?.GetId());
		Assert.Equal(expected, actual, _cmp);
	}

	[Fact(DisplayName = "Should lookup posts with Content and Creators")]
	public async Task CanLookupWithNavigations()
	{
		var expected = _posts[_profiles[0]][0];
		var actual = await _adapter.LookupPost(expected.GetId());

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.Contents);
		Assert.NotEmpty(actual.Creators);
	}

	[Fact(DisplayName = "Should not lookup non-existent posts")]
	public async Task CanLookupMissing()
	{
		var actual = await _adapter.LookupPost(Uuid7.NewUuid7());

		Assert.Null(actual);
	}

	[Fact(DisplayName = "Should lookup threads")]
	public async Task CanLookupThread()
	{
		var expected = _posts[_profiles[0]][0].Thread;
		var actual = await _adapter.LookupThread(expected.GetId());

		Assert.NotNull(actual);
		Assert.Equal(expected.GetId(), actual.GetId());
		Assert.Equal(expected.FediId, actual.FediId);
	}

	[Fact(DisplayName = "Should lookup threads by FediID")]
	public async Task CanLookupThreadByFediId()
	{
		var expected = _posts[_profiles[0]][0].Thread;
		var actual = await _adapter.LookupThread(expected.FediId);

		Assert.NotNull(actual);
		Assert.Equal(expected.GetId(), actual.GetId());
		Assert.Equal(expected.FediId, actual.FediId);
	}

	[Fact(DisplayName = "Should lookup threads with posts")]
	public async Task CanLookupThreadAndPosts()
	{
		// Should have several posts in this thread
		var expected = _posts[_profiles[0]][3].Thread;
		var actual = await _adapter.LookupThread(expected.GetId());

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.Posts);
		Assert.Contains(_posts[_profiles[0]][2], actual.Posts, _cmp);
		Assert.Contains(_posts[_profiles[4]][0], actual.Posts, _cmp);
	}

	[Fact(DisplayName = "Should lookup a post with its thread")]
	public async Task CanLookupPostAndThread()
	{
		// Should have several posts in this thread
		var expected = _posts[_profiles[0]][3];
		var actual = await _adapter.LookupPostWithThread(expected.GetId());

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.Thread.Posts);
		Assert.Contains(_posts[_profiles[0]][2], actual.Thread.Posts, _cmp);
		Assert.Contains(_posts[_profiles[4]][0], actual.Thread.Posts, _cmp);
	}

	[Fact(DisplayName = "Should not lookup non-existent threads")]
	public async Task CanLookupMissingThread()
	{
		var actual = await _adapter.LookupThread(Uuid7.NewUuid7());

		Assert.Null(actual);
	}
}