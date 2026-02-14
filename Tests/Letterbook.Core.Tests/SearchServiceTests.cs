using Letterbook.Core.Adapters;
using Letterbook.Core.Tests.Fakes;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class SearchServiceTests : WithMocks
{
	private readonly SearchService _service;
	private List<ISearchProvider> _providers = [];
	private FakeProfile _profiles;

	public SearchServiceTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_profiles = new FakeProfile();
		_service = new SearchService(_providers, AuthorizationServiceMock.Object);

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
			.ReturnsAsync(_profiles.Generate(1));
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
				.ReturnsAsync(_profiles.Generate(1));
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
			.ReturnsAsync(_profiles.Generate(5));
		_providers.AddRange(mocks.Select(m => m.Object));

		var actual = await _service.As([]).SearchProfiles("query", CancellationToken.None, 1).ToListAsync();

		Assert.Equal(5, actual.Count);
	}
}