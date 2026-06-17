using Letterbook.Core;
using Letterbook.Core.Adapters;
using Moq;

namespace Letterbook.Adapter.ActivityPub.Test;

public class FallbackSearchProviderTests
{
	[Fact]
	public async Task SearchProfilesFallsBackToSecondaryWhenPrimaryReturnsEmpty()
	{
		var primary = new Mock<ISearchProvider>(MockBehavior.Strict);
		var secondary = new Mock<ISearchProvider>();

		primary
			.Setup(
				it => it.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
			.ReturnsAsync([]);

		var fallbackSearchProvider = new FallbackSearchProvider(primary.Object, secondary.Object);

		await fallbackSearchProvider.SearchProfiles("abc", CancellationToken.None, new CoreOptions());

		secondary.Verify(it => it.SearchProfiles("abc", CancellationToken.None, It.IsAny<CoreOptions>(), It.IsAny<int>()));
	}

	[Fact]
	public async Task SearchProfilesAllowsSecondaryToBeNull()
	{
		var primary = new Mock<ISearchProvider>(MockBehavior.Strict);

		primary
			.Setup(
				it => it.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
			.ReturnsAsync([]);

		var fallbackSearchProvider = new FallbackSearchProvider(primary.Object, null as ISearchProvider);

		var result = await fallbackSearchProvider.SearchProfiles("abc", CancellationToken.None, new CoreOptions());

		Assert.Empty(result);
	}

	[Fact]
	public async Task SearchProfilesDoesNotInvokeSecondaryWhenPrimaryReturnsNonEmpty()
	{
		var primary = new Mock<ISearchProvider>(MockBehavior.Strict);
		var secondary = new Mock<ISearchProvider>(MockBehavior.Strict);

		var expectedProfile = Models.Profile.CreateEmpty(new Uri("acct:handle@authority"));

		primary
			.Setup(
				it => it.SearchProfiles(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
			.ReturnsAsync([ expectedProfile ]);

		var fallbackSearchProvider = new FallbackSearchProvider(primary.Object, secondary.Object);

		var result = await fallbackSearchProvider.SearchProfiles("abc", CancellationToken.None, new CoreOptions());

		var actualProfile = Assert.Single(result);

		Assert.Same(expectedProfile, actualProfile);
	}

	[Fact]
	public async Task SearchAnyDoesNotFallBack()
	{
		var primary = new Mock<ISearchProvider>(MockBehavior.Strict);
		var secondary = new Mock<ISearchProvider>(MockBehavior.Strict);

		primary
			.Setup(
				it => it.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
			.ReturnsAsync([ ]);

		var fallbackSearchProvider = new FallbackSearchProvider(primary.Object, secondary.Object);

		var result = await fallbackSearchProvider.SearchAny("abc", CancellationToken.None, new CoreOptions());

		Assert.Empty(result);
	}

	[Fact]
	public async Task SearchPostsDoesNotFallBack()
	{
		var primary = new Mock<ISearchProvider>(MockBehavior.Strict);
		var secondary = new Mock<ISearchProvider>(MockBehavior.Strict);

		primary
			.Setup(
				it => it.SearchAny(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<CoreOptions>(), It.IsAny<int>()))
			.ReturnsAsync([ ]);

		var fallbackSearchProvider = new FallbackSearchProvider(primary.Object, secondary.Object);

		var result = await fallbackSearchProvider.SearchAny("abc", CancellationToken.None, new CoreOptions());

		Assert.Empty(result);
	}
}