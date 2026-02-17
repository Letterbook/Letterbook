using System.Net;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.IntegrationTests.Fixtures;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

public class ProfileLookupTests : IClassFixture<ApiFixture>
{
	private readonly ApiFixture _fixture;
	private readonly ITestOutputHelper _log;

	public ProfileLookupTests(ApiFixture fixture, ITestOutputHelper log)
	{
		_fixture = fixture;
		_log = log;
	}

	[Fact(DisplayName = "Should invoke search profiles")]
	public async Task InvokeCorrectSearchMethod()
	{
		var mockSearchProvider = new Mock<ISearchProvider>(MockBehavior.Default);

		mockSearchProvider.Setup(it => it.SearchProfiles(
			It.IsAny<string>(),
			It.IsAny<CancellationToken>(),
			It.IsAny<CoreOptions>(),
			It.IsAny<int>())).ReturnsAsync(new List<Models.Profile>
			{
				Models.Profile.CreateIndividual(new Uri("acct:letterbook.social"), "ben")
			});

		_fixture.ReplaceScoped(mockSearchProvider.Object);

		using var _client = _fixture.CreateClient();

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@letterbook.social");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		mockSearchProvider
			.Verify(it => it.SearchProfiles(
				"ben@letterbook.social",
				It.IsAny<CancellationToken>(),
				It.IsAny<CoreOptions>(), // What is core options for? What value shoud it have?
				100));
	}

	// TEST: is authentication required?
}