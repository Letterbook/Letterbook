using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace Letterbook.IntegrationTests;

public class ProfileLookupExamples(HostFixture<ProfileLookupExamples> hostFixture): IClassFixture<HostFixture<ProfileLookupExamples>>, ITestSeed {
	private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
	{
		Converters = { new Uuid7JsonConverter() },
		ReferenceHandler = ReferenceHandler.IgnoreCycles
	};
	
	[Fact(DisplayName = "Should use webfinger for external profile")]
	public async Task FallBackToWebFinger()
	{
		var externalProfile = Models.Profile.CreateEmpty(new Uri("acct:ben@mastodon.social"));
		externalProfile.Handle = "ben";

		hostFixture
			.MockActivityPubClient.Setup(it => it.Fetch<Models.Profile>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(externalProfile);

		using var _client = hostFixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

		_client.DefaultRequestHeaders.Authorization = new("Test", $"{hostFixture.Accounts[0].Id}");

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@mastodon.social");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var actual = Assert.IsType<FullProfileDto[]>(await response.Content.ReadFromJsonAsync<FullProfileDto[]>(_json));

		var actualProfile = Assert.Single(actual);

		Assert.Equal("ben", actualProfile.Handle);
	}


	[Fact(DisplayName = "Should return empty for unknown external profile")]
	public async Task ReturnEmptyForUnknownProfile()
	{
		var unknownProfile = Models.Profile.CreateEmpty(new Uri("acct:xxx@xxx.unknown.xxx"));

		hostFixture
			.MockActivityPubClient.Setup(it => it.Fetch<Models.Profile>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(unknownProfile);

		using var _client = hostFixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

		_client.DefaultRequestHeaders.Authorization = new("Test", $"{hostFixture.Accounts[0].Id}");

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=xxx@xxx.unknown.xxx");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var actual = Assert.IsType<FullProfileDto[]>(await response.Content.ReadFromJsonAsync<FullProfileDto[]>(_json));

		Assert.Empty(actual);
	}

	[Fact(DisplayName = "Should return local profile")]
	public async Task ReturnLocalProfile()
	{
		var localProfile = hostFixture.Profiles[0];

		hostFixture
			.MockActivityPubClient.Setup(it => it.Fetch<Models.Profile>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(localProfile);

		using var _client = hostFixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

		_client.DefaultRequestHeaders.Authorization = new("Test", $"{hostFixture.Accounts[0].Id}");

		var response = await _client.GetAsync($"/lb/v1/search_profiles?q={localProfile.Handle}@{localProfile.Authority}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var actual = Assert.IsType<FullProfileDto[]>(await response.Content.ReadFromJsonAsync<FullProfileDto[]>(_json));

		var actualProfile = Assert.Single(actual);

		Assert.Equal(localProfile.Handle, actualProfile.Handle);
	}
}