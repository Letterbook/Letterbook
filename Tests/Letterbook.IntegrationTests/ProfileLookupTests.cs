using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Letterbook.IntegrationTests;

public class ProfileLookupTests(ProfileLookupFixture fixture, ITestOutputHelper log) : IClassFixture<ProfileLookupFixture>, ITestSeed
{
	private readonly ITestOutputHelper _log = log;
	private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
	{
		Converters = { new Uuid7JsonConverter() },
		ReferenceHandler = ReferenceHandler.IgnoreCycles
	};

	[Fact(DisplayName = "Should invoke search profiles")]
	public async Task InvokeCorrectSearchMethod()
	{
		var expectedProfile = Models.Profile.CreateIndividual(new Uri("acct:letterbook.social"), "ben");

		fixture.MockSearchProvider.Setup(it => it.SearchProfiles(
			It.IsAny<string>(),
			It.IsAny<CancellationToken>(),
			It.IsAny<CoreOptions>(),
			It.IsAny<int>())).ReturnsAsync(new List<Models.Profile>
			{
				expectedProfile
			});

		using var _client = fixture.CreateClient();

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@letterbook.social");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		fixture.MockSearchProvider
			.Verify(it => it.SearchProfiles(
				"ben@letterbook.social",
				It.IsAny<CancellationToken>(),
				It.IsAny<CoreOptions>(), // What is core options for? What value should it have?
				100));

		var actual = Assert.IsType<FullProfileDto[]>(await response.Content.ReadFromJsonAsync<FullProfileDto[]>(_json));

		var actualProfile = Assert.Single(actual);

		Assert.Equal(expectedProfile.Handle, actualProfile.Handle);
	}

	[Fact(DisplayName = "Should return empty list when nothing is found")]
	public async Task ReturnEmptyWhenNothingFound()
	{
		fixture.MockSearchProvider.Setup(it => it.SearchProfiles(
			It.IsAny<string>(),
			It.IsAny<CancellationToken>(),
			It.IsAny<CoreOptions>(),
			It.IsAny<int>())).ReturnsAsync([]);

		using var _client = fixture.CreateClient();

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@letterbook.social");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var actual = Assert.IsType<FullProfileDto[]>(await response.Content.ReadFromJsonAsync<FullProfileDto[]>(_json));

		Assert.Empty(actual);
	}

	[Fact(DisplayName = "Should return HTTP 500 when search throws exception")]
	public async Task Return500WhenSearchThrowsException()
	{
		fixture.MockSearchProvider.Setup(it => it.SearchProfiles(
			It.IsAny<string>(),
			It.IsAny<CancellationToken>(),
			It.IsAny<CoreOptions>(),
			It.IsAny<int>())).ThrowsAsync(new Exception("Profile search failed on purpose."));

		using var _client = fixture.CreateClient();

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@letterbook.social");

		Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

		Assert.Contains("Profile search failed on purpose", await response.Content.ReadAsStringAsync());
	}

	[Fact(DisplayName = "Should require authorization")]
	public async Task RejectUnauthorizedRequest()
	{
		using var _client = fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

		_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("None");

		var response = await _client.GetAsync("/lb/v1/search_profiles?q=ben@letterbook.social");

		// @todo: Should this really return 401 instead of redirecting to log-in?
		Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
		Assert.Equal("/Identity/Account/Login", response.Headers.Location!.AbsolutePath);
	}

	[Fact(DisplayName = "Should use webfinger for external profile")]
	public async Task FallBackToWebFinger()
	{
		var externalProfile = Models.Profile.CreateEmpty(new Uri("acct:ben@mastodon.social"));
		externalProfile.Handle = "ben";

		await using var hostFixture = new HostFixture<ProfileLookupTests>(new NullMessageSink());

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

		await using var hostFixture = new HostFixture<ProfileLookupTests>(new NullMessageSink());

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
	// TEST: [!] searching for local profile should return it
	// TEST: what happens if you supply 'q' more than once?
}

// ReSharper disable once ClassNeverInstantiated.Global
public class ProfileLookupFixture : ApiFixture
{
	public Mock<ISearchProvider> MockSearchProvider { get; } = new(MockBehavior.Strict);

	public ProfileLookupFixture()
	{
		ReplaceScoped(MockSearchProvider.Object);
	}
}