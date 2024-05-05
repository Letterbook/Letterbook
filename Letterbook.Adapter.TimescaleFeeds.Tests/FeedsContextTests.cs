using Letterbook.Adapter.TimescaleFeeds._Tests.Fixtures;
using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Xunit.Abstractions;

namespace Letterbook.Adapter.TimescaleFeeds._Tests;

/// <summary>
/// Tests that the DbContext is properly configured and can handle our entity models.
/// These tests don't require an actual database, as long as they don't actually query or save changes made to the context.
///
/// Essentially that means we expect the context actions not to throw. These tests are fast and helpful, but can't replace testing
/// integration with an actual database.
/// </summary>
public class FeedsContextTests : IClassFixture<TimescaleFixture<FeedsContextTests>>, IDisposable
{
	public void Dispose() => _context.Dispose();

	private readonly FeedsContext _context;
	private readonly FakePost _fakePost;
	private readonly Post _post;

	public FeedsContextTests(ITestOutputHelper output, TimescaleFixture<FeedsContextTests> timescale)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_fakePost = new FakePost("letterbook.example");
		_post = _fakePost.Generate();
		_context = timescale.CreateContext();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_context);
	}

	[Fact(DisplayName = "Should track Posts")]
	public void CanTrackPost()
	{
		var actual = TimelinePost.ToIEnumerable(_post);

		Assert.True(actual.Any());
		_context.Timelines.AddRange(actual);
	}

	[Fact(DisplayName = "Should track Notifications", Skip = "Not implemented")]
	public void CanTrackNotifications()
	{
		Assert.Fail("Not implemented");
	}
}