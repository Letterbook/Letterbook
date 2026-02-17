using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.IntegrationTests.Fixtures;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests;

public class ProfileLookupTests(ApiFixture fixture, ITestOutputHelper log) : IClassFixture<ApiFixture>
{
	private readonly ITestOutputHelper _log = log;

	[Fact(DisplayName = "Should invoke search profiles")]
	public async Task InvokeCorrectSearchMethod()
	{
		var mockSearchProvider = new Mock<ISearchProvider>(MockBehavior.Default);

		var expectedProfile = Models.Profile.CreateIndividual(new Uri("acct:letterbook.social"), "ben");

		mockSearchProvider.Setup(it => it.SearchProfiles(
			It.IsAny<string>(),
			It.IsAny<CancellationToken>(),
			It.IsAny<CoreOptions>(),
			It.IsAny<int>())).ReturnsAsync(new List<Models.Profile>
			{
				expectedProfile
			});

		fixture.ReplaceScoped(mockSearchProvider.Object);

		using var _client = fixture.CreateClient();

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@letterbook.social");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		mockSearchProvider
			.Verify(it => it.SearchProfiles(
				"ben@letterbook.social",
				It.IsAny<CancellationToken>(),
				It.IsAny<CoreOptions>(), // What is core options for? What value should it have?
				100));

		var json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

		var actual = Assert.IsType<FullProfileDto[]>(await response.Content.ReadFromJsonAsync<FullProfileDto[]>(json));

		var actualProfile = Assert.Single(actual);

		Assert.Equal(expectedProfile.Handle, actualProfile.Handle);
	}

	// TEST: is authentication required?
}