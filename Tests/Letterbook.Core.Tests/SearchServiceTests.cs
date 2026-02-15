using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class SearchServiceTests : WithMocks
{
	private readonly SearchService _service;
	private List<ISearchProvider> _providers = [];
	private FakeProfile _localProfiles;
	private FakeProfile _peerProfiles;
	private readonly FakePost _localPosts;
	private readonly FakePost _peerPosts;

	public SearchServiceTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_localProfiles = new FakeProfile(CoreOptionsMock.Value.DomainName);
		_localPosts = new FakePost(CoreOptionsMock.Value.DomainName);
		_peerProfiles = new FakeProfile();
		_peerPosts = new FakePost("peer.example");
		_service = new SearchService(Mock.Of<ILogger<SearchService>>(), _providers, AuthorizationServiceMock.Object, DataAdapterMock.Object,
			CoreOptionsMock);

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_service);
	}

	/***
	 * SearchProfiles
	 */
	[Fact(DisplayName = "SearchProfiles should call all providers")]
	public async Task ProfileShouldCallAllProviders()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None).ToListAsync();

		foreach (var mock in mocks)
		{
			mock.Verify(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Once);
		}
	}

	[Fact(DisplayName = "SearchProfiles should stop calling providers after limit reached")]
	public async Task ProfileShouldStopAfterLimitReached()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		mocks[0].Setup(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_localProfiles.Generate(1));
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None, 1).ToListAsync();

		mocks[0].Verify(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Once);
		mocks[1].Verify(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Never);
	}

	[Fact(DisplayName = "SearchProfiles should combine results from all providers")]
	public async Task ProfileShouldCombineResultsFromAllProviders()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new(), new() };
		foreach (var mock in mocks.Skip(1))
		{
			mock.Setup(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
				.ReturnsAsync(_localProfiles.Generate(1));
		}
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None).ToListAsync();

		Assert.Equal(2, actual.Count);
	}

	[Fact(DisplayName = "SearchProfiles should not truncate already retrieved results beyond the limit")]
	public async Task ProfileShouldNotTruncateAlreadyRetrievedResults()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		mocks[0].Setup(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_localProfiles.Generate(5));
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None, 1).ToListAsync();

		Assert.Equal(5, actual.Count);
	}

	[Fact(DisplayName = "SearchProfiles should exit early on local results")]
	public async Task ProfilesShouldExitLocal()
    	{
    		var profiles = _localProfiles.Generate(1);
    		var provider = new Mock<ISearchProvider>();
    		var otherProvider = new Mock<ISearchProvider>();
    		provider.Setup(p => p.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
    			.ReturnsAsync(profiles);
    		_providers.AddRange(provider.Object, otherProvider.Object);

    		var actual = await _service.As([]).SearchProfiles($"@{profiles[0].Handle}", CancellationToken.None).ToListAsync();
    		Assert.Single(actual);

    		otherProvider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Never);
    	}

	[Fact(DisplayName = "SearchProfiles should add profiles")]
	public async Task ProfilesShouldAddProfileToDb()
	{
		var provider = new Mock<ISearchProvider>();
		provider.Setup(p => p.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_peerProfiles.Generate(1));
		_providers.AddRange(Mock.Of<ISearchProvider>(), provider.Object);

		await _service.As([]).SearchProfiles("query", CancellationToken.None).ToListAsync();

		DataAdapterMock.Verify(m => m.Add(It.IsAny<Profile>()), Times.Once);
	}

	/***
	 * SearchPosts
	 */
	[Fact(DisplayName = "SearchPosts should call all providers")]
	public async Task PostsShouldCallAllProviders()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchPosts("query", CancellationToken.None).ToListAsync();

		foreach (var mock in mocks)
		{
			mock.Verify(m => m.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Once);
		}
	}

	[Fact(DisplayName = "SearchPosts should stop calling providers after limit reached")]
	public async Task PostsShouldStopAfterLimitReached()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		mocks[0].Setup(m => m.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_localPosts.Generate(1));
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchPosts("query", CancellationToken.None, 1).ToListAsync();

		mocks[0].Verify(m => m.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Once);
		mocks[1].Verify(m => m.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Never);
	}

	[Fact(DisplayName = "SearchPosts should combine results from all providers")]
	public async Task PostsShouldCombineResultsFromAllProviders()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new(), new() };
		foreach (var mock in mocks.Skip(1))
		{
			mock.Setup(m => m.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
				.ReturnsAsync(_localPosts.Generate(1));
		}
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchPosts("query", CancellationToken.None).ToListAsync();

		Assert.Equal(2, actual.Count);
	}

	[Fact(DisplayName = "SearchPosts should not truncate already retrieved results beyond the limit")]
	public async Task PostsShouldNotTruncateAlreadyRetrievedResults()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		mocks[0].Setup(m => m.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_localPosts.Generate(5));
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchPosts("query", CancellationToken.None, 1).ToListAsync();

		Assert.Equal(5, actual.Count);
	}

	[Fact(DisplayName = "SearchPosts should exit early on local results")]
	public async Task PostsShouldExitLocal()
    	{
    		var posts = _localPosts.Generate(1);
    		var provider = new Mock<ISearchProvider>();
    		var otherProvider = new Mock<ISearchProvider>();
    		provider.Setup(p => p.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
    			.ReturnsAsync(posts);
    		_providers.AddRange(provider.Object, otherProvider.Object);

    		var actual = await _service.As([]).SearchPosts("query", CancellationToken.None).ToListAsync();
    		Assert.Single(actual);

    		otherProvider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Never);
    	}

	[Fact(DisplayName = "SearchPosts should add posts")]
	public async Task PostsShouldAddToDb()
	{
		var provider = new Mock<ISearchProvider>();
		provider.Setup(p => p.SearchPosts(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_localPosts.Generate(1));
		_providers.AddRange(Mock.Of<ISearchProvider>(), provider.Object);

		await _service.As([]).SearchPosts("query", CancellationToken.None).ToListAsync();

		DataAdapterMock.Verify(m => m.Add(It.IsAny<Post>()), Times.Once);
	}

	/***
	 * SearchAll
	 */

	[Fact(DisplayName = "SearchAll should exit early on local results")]
	public async Task AllShouldExitLocal()
	{
		var profiles = _localProfiles.Generate(1);
		var provider = new Mock<ISearchProvider>();
		var otherProvider = new Mock<ISearchProvider>();
		provider.Setup(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
			.ReturnsAsync(profiles);
		_providers.AddRange(provider.Object, otherProvider.Object);

		var actual = await _service.As([]).SearchAll($"@{profiles[0].Handle}", CancellationToken.None).ToListAsync();
		Assert.Single(actual);

		otherProvider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Never);
	}

	[Fact(DisplayName = "SearchAll should call providers")]
	public async Task AllShouldCallProviders()
	{
		var provider = new Mock<ISearchProvider>();
		_providers.AddRange(Mock.Of<ISearchProvider>(), provider.Object);

		await _service.As([]).SearchAll("any query text", CancellationToken.None).ToListAsync();

		provider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()), Times.Once);
	}

	[Fact(DisplayName = "SearchAll should add profiles")]
	public async Task AllShouldAddProfileToDb()
	{
		var provider = new Mock<ISearchProvider>();
		provider.Setup(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_peerProfiles.Generate(1));
		_providers.AddRange(Mock.Of<ISearchProvider>(), provider.Object);

		await _service.As([]).SearchAll("query", CancellationToken.None).ToListAsync();

		DataAdapterMock.Verify(m => m.Add(It.IsAny<Profile>()), Times.Once);
	}

	[Fact(DisplayName = "SearchAll should add posts")]
	public async Task AllShouldAddPostsToDb()
	{
		var provider = new Mock<ISearchProvider>();
		provider.Setup(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), CoreOptionsMock.Value, It.IsAny<int>()))
			.ReturnsAsync(_peerPosts.Generate(1));
		_providers.AddRange(Mock.Of<ISearchProvider>(), provider.Object);

		await _service.As([]).SearchAll("query", CancellationToken.None).ToListAsync();

		DataAdapterMock.Verify(m => m.Add(It.IsAny<Post>()), Times.Once);
	}
}