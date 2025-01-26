using Letterbook.Adapter.Db;
using Letterbook.Core.Models;
using Letterbook.IntegrationTests.Fixtures;
using Medo;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Adapter")]
public sealed class PostAdapterTests : IClassFixture<HostFixture<PostAdapterTests>>, ITestSeed, IDisposable
{
	public void Dispose()
	{
		_scope.Dispose();
	}

	private readonly ITestOutputHelper _output;
	private readonly HostFixture<PostAdapterTests> _host;
	private DataAdapter _adapter;
	private RelationalContext _context;
	private RelationalContext _actual;
	private Dictionary<Profile, List<Post>> _posts;
	private List<Profile> _profiles;
	private static ValueComparer<Post> _cmp;
	private readonly IServiceScope _scope;
	static int? ITestSeed.Seed() => null;

	public PostAdapterTests(ITestOutputHelper output, HostFixture<PostAdapterTests> host)
	{
		_output = output;
		_host = host;
		_posts = host.Posts;
		_profiles = host.Profiles;
		_scope = host.CreateScope();

		_context = host.CreateContext(_scope);
		_actual = host.CreateContext(_scope);
		_adapter = new DataAdapter(Mock.Of<ILogger<DataAdapter>>(), _context);
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

	[Fact(DisplayName = "Should lookup threads")]
	public async Task CanLookupThread()
	{
		var expected = _posts[_profiles[0]][0].Thread;
		var actual = await _adapter.LookupThread(expected.Id);

		Assert.NotNull(actual);
		Assert.Equal(expected.Id, actual.Id);
		Assert.Equal(expected.FediId, actual.FediId);
	}

	[Fact(DisplayName = "Should lookup threads by FediID")]
	public async Task CanLookupThreadByFediId()
	{
		var expected = _posts[_profiles[0]][0].Thread;
		var actual = await _adapter.LookupThread(expected.FediId!);

		Assert.NotNull(actual);
		Assert.Equal(expected.Id, actual.Id);
		Assert.Equal(expected.FediId, actual.FediId);
	}

	[Fact(DisplayName = "Should lookup threads with posts")]
	public async Task CanLookupThreadAndPosts()
	{
		// Should have several posts in this thread
		var expected = _posts[_profiles[0]][3].Thread;
		var actual = await _adapter.LookupThread(expected.Id);

		Assert.NotNull(actual);
		Assert.NotEmpty(actual.Posts);
		Assert.Contains(_posts[_profiles[0]][2], actual.Posts, _cmp);
		Assert.Contains(_posts[_profiles[4]][0], actual.Posts, _cmp);
	}

	[Fact(DisplayName = "Should not lookup non-existent threads")]
	public async Task CanLookupMissingThread()
	{
		var actual = await _adapter.LookupThread(Uuid7.NewUuid7());

		Assert.Null(actual);
	}
}