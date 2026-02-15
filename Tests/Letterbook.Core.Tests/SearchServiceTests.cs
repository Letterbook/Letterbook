using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Logging;
using MockQueryable;
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

	[Fact]
	public async Task ProfileShouldCallAllProviders()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None).ToListAsync();

		foreach (var mock in mocks)
		{
			mock.Verify(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
		}
	}

	[Fact]
	public async Task ProfileShouldStopAfterLimitReached()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		mocks[0].Setup(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
			.ReturnsAsync(_localProfiles.Generate(1));
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None, 1).ToListAsync();

		mocks[0].Verify(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
		mocks[1].Verify(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
	}

	[Fact]
	public async Task ProfileShouldCombineResultsFromAllProviders()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		foreach (var mock in mocks)
		{
			mock.Setup(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
				.ReturnsAsync(_localProfiles.Generate(1));
		}
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None).ToListAsync();

		Assert.Equal(2, actual.Count);
	}

	[Fact]
	public async Task ProfileShouldNotTruncateAlreadyRetrievedResults()
	{
		var mocks = new List<Mock<ISearchProvider>> { new(), new() };
		mocks[0].Setup(m => m.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
			.ReturnsAsync(_localProfiles.Generate(5));
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None, 1).ToListAsync();

		Assert.Equal(5, actual.Count);
	}

	/***
	 * SearchAll
	 */

	[Fact(DisplayName = "SearchAll should exit early on local handle")]
	public async Task AllShouldExitLocal()
	{
		var profiles = _localProfiles.Generate(1);
		var provider = new Mock<ISearchProvider>();
		_providers.Add(provider.Object);
		DataAdapterMock.Setup(m => m.AllProfiles()).Returns(profiles.BuildMock());

		var actual = await _service.As([]).SearchAll($"@{profiles[0].Handle}", CancellationToken.None).ToListAsync();
		Assert.Single(actual);

		provider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
	}

	[Fact(DisplayName = "SearchAll should exit early on full handle")]
	public async Task AllShouldExitFullHandle()
	{
		var profiles = _localProfiles.Generate(1);
		var provider = new Mock<ISearchProvider>();
		_providers.Add(provider.Object);
		DataAdapterMock.Setup(m => m.AllProfiles()).Returns(profiles.BuildMock());

		var actual = await _service.As([]).SearchAll($"@{profiles[0].Handle}@{CoreOptionsMock.Value.DomainName}", CancellationToken.None).ToListAsync();
		Assert.Single(actual);

		provider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
	}

	[Fact(DisplayName = "SearchAll should exit early on profile Uri")]
	public async Task AllShouldExitProfileUri()
	{
		var profiles = _localProfiles.Generate(1);
		var provider = new Mock<ISearchProvider>();
		_providers.Add(provider.Object);
		DataAdapterMock.Setup(m => m.Profiles(It.IsAny<Uri[]>())).Returns(profiles.BuildMock());

		var actual = await _service.As([]).SearchAll(profiles[0].FediId.ToString(), CancellationToken.None).ToListAsync();
		Assert.Single(actual);

		provider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
	}

	[Fact(DisplayName = "SearchAll should exit early on post Uri")]
	public async Task AllShouldExitPostUri()
	{
		var posts = _localPosts.Generate(1);
		var provider = new Mock<ISearchProvider>();
		_providers.Add(provider.Object);
		DataAdapterMock.Setup(m => m.Posts(It.IsAny<Uri[]>())).Returns(posts.BuildMock());

		var actual = await _service.As([]).SearchAll(posts[0].FediId.ToString(), CancellationToken.None).ToListAsync();
		Assert.Single(actual);

		provider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
	}

	[InlineData("@handle")]
	[InlineData("@handle@host")]
	[InlineData("@handle@host.example")]
	[InlineData("@handle@host.example@still.going")]
	[InlineData("https://host.example/value")]
	[InlineData("any other text")]
	[Theory(DisplayName = "SearchAll should call providers")]
	public async Task AllShouldCallProviders(string query)
	{
		var provider = new Mock<ISearchProvider>();
		_providers.Add(provider.Object);

		await _service.As([]).SearchAll(query, CancellationToken.None).ToListAsync();

		provider.Verify(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
	}

	[Fact(DisplayName = "SearchAll should add profiles")]
	public async Task AllShouldAddProfileToDb()
	{
		var provider = new Mock<ISearchProvider>();
		provider.Setup(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
			.ReturnsAsync(_peerProfiles.Generate(1));
		_providers.Add(provider.Object);

		await _service.As([]).SearchAll("query", CancellationToken.None).ToListAsync();

		DataAdapterMock.Verify(m => m.Add(It.IsAny<Profile>()), Times.Once);
	}

	[Fact(DisplayName = "SearchAll should add posts")]
	public async Task AllShouldAddPostsToDb()
	{
		var provider = new Mock<ISearchProvider>();
		provider.Setup(p => p.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
			.ReturnsAsync(_peerPosts.Generate(1));
		_providers.Add(provider.Object);

		await _service.As([]).SearchAll("query", CancellationToken.None).ToListAsync();

		DataAdapterMock.Verify(m => m.Add(It.IsAny<Post>()), Times.Once);
	}
}