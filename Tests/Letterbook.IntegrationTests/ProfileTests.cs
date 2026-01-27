using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Letterbook.Api.Tests.Fakes;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.Core.Values;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.IntegrationTests;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Api")]
public sealed class ProfileTests : IClassFixture<HostFixture<ProfileTests>>, ITestSeed, IDisposable
{
	public void Dispose()
	{
		_scope.Dispose();
	}

	private readonly HostFixture<ProfileTests> _host;
	private readonly ITestOutputHelper _output;
	private readonly HttpClient _client;
	private readonly List<Profile> _profiles;
	private readonly Dictionary<Profile, List<Post>> _posts;
	private readonly FakePostDto _postDto;
	private readonly Mapper _mapper;
	private readonly JsonSerializerOptions _json;
	private readonly List<Account> _accounts;
	private readonly IServiceScope _scope;
	static int? ITestSeed.Seed() => null;

	public ProfileTests(HostFixture<ProfileTests> host, ITestOutputHelper output)
	{
		_host = host;
		_output = output;
		_scope = host.CreateScope();
		var clientOptions = new WebApplicationFactoryClientOptions
		{
			BaseAddress = _host.Options?.BaseUri() ?? new Uri("localhost:5127"),
			AllowAutoRedirect = false
		};
		_client = _host.CreateClient(clientOptions);
		_client.DefaultRequestHeaders.Authorization = new("Test", $"{_host.Accounts[0].Id}");

		_profiles = _host.Profiles;
		_accounts = _host.Accounts;
		_posts = _host.Posts;
		_postDto = new FakePostDto(_profiles[0]);
		var profileMappings = _scope.ServiceProvider.GetRequiredService<MappingConfigProvider>().Profiles;
		_mapper = new Mapper(profileMappings);
		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};

	}

	private bool CustomFieldComparer(CustomField? l, CustomField? r)
	{
		return l != null && r != null && l.Label == r.Label && l.Value == r.Value;
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
	}

	[Fact(DisplayName = "Should get a profile by ID")]
	public async Task CanGetProfile()
	{
		var response = await _client.GetAsync($"/lb/v1/profiles/{_profiles[0].GetId25()}");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));
		Assert.Equal(_profiles[0].Handle, actual.Handle);
	}

	/*

		https://docs.joinmastodon.org/spec/webfinger

		@todo: What do we do about the URL prefix '/lb/v1'? Is that hidden at runtime?
		@todo: what does the shape of the reply look like? FullProfileDto or other?
		@todo: there should be no authorization required
		@todo: Is there a specific shape that the resource parameter needs to take?
	*/
	[Fact(DisplayName = "Should get a profile by web finger")]
	public async Task CanGetProfileByWebFinger()
	{
		var profile = _profiles[0];

		var response = await _client.GetAsync($"/lb/v1/.well-known/webfinger?resource={profile.Handle}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var body = await response.Content.ReadAsStringAsync();

		_output.WriteLine(body);

		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));

		Assert.Equal(profile.Handle, actual.Handle);
	}

	[Fact(DisplayName = "Should return 404 when no handle found")]
	public async Task WebFingerReturns404NotFoundWhenHandleDoesNotExist()
	{
		var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/lb/v1/.well-known/webfinger?resource=xxx-does-not-exist-xxx")
		{
			Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }}
		});

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

		// [!] Don't really expect this as I asked for "application/json"
		Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
	}

	[Fact(DisplayName = "Should create a profile owned by an actor")]
	public async Task CanCreateProfile()
	{
		var account = _accounts[0];
		var expected = "test_profile";

		var response = await _client.PostAsync($"/lb/v1/profiles/new/{account.Id}?handle={expected}", null);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));
		Assert.Equal(expected, actual.Handle);
	}

	[Fact(DisplayName = "Should add a custom field to a profile")]
	public async Task CanAddField()
	{
		var expected = new CustomField
		{
			Label = "New field",
			Value = "Test value"
		};
		var response = await _client.PostAsJsonAsync($"/lb/v1/profiles/{_profiles[1].GetId25()}/field/0", expected);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));
		Assert.Equal(expected, actual.CustomFields?[0], CustomFieldComparer);
	}

	[Fact(DisplayName = "Should remove a custom field from a profile")]
	public async Task CanRemoveField()
	{
		var expected = _profiles[2].CustomFields;
		var response = await _client.DeleteAsync($"/lb/v1/profiles/{_profiles[2].GetId25()}/field/0");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));
		Assert.NotEqual(expected.Length, actual.CustomFields?.Length);
	}

	[Fact(DisplayName = "Should update a custom field on a profile")]
	public async Task CanUpdateField()
	{
		var expected = new CustomField
		{
			Label = _profiles[0].CustomFields[0].Label,
			Value = "Test value"
		};
		var response = await _client.PutAsJsonAsync($"/lb/v1/profiles/{_profiles[0].GetId25()}/field/0", expected);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));
		Assert.Equal(expected, actual.CustomFields?[0], CustomFieldComparer);
	}

	[Fact(DisplayName = "Should update a whole profile", Skip = "not implemented")]
	public async Task CanUpdateEdit()
	{
		var profile = _profiles[3];
		var dto = _mapper.Map<FullProfileDto>(profile);
		dto.Description = "updated description";
		dto.DisplayName = "updated displayname";
		dto.CustomFields = [];
		var response = await _client.PutAsJsonAsync($"/lb/v1/profiles/{_profiles[0].GetId25()}", dto, _json);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullProfileDto>(await response.Content.ReadFromJsonAsync<FullProfileDto>(_json));
		Assert.Equal(dto, actual);
	}

	[Trait("Group", "Follow")]
	[Fact(DisplayName = "Should query followers")]
	public async Task CanGetFollowers()
	{
		var P1 = _profiles[1];
		var P5 = _profiles[5];
		var path = $"/lb/v1/profiles/{P5.GetId25()}/follower";
		var response = await _client.GetAsync(path);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsAssignableFrom<IEnumerable<MiniProfileDto>>(
			await response.Content.ReadFromJsonAsync<IEnumerable<MiniProfileDto>>(_json));

		Assert.Contains(actual, dto => dto.Id == P1.GetId());
	}

	[Trait("Group", "Follow")]
	[Fact(DisplayName = "Should query following")]
	public async Task CanGetFollows()
	{
		var P1 = _profiles[1];
		var P5 = _profiles[5];
		var path = $"/lb/v1/profiles/{P1.GetId25()}/following";
		var response = await _client.GetAsync(path);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsAssignableFrom<IEnumerable<MiniProfileDto>>(
			await response.Content.ReadFromJsonAsync<IEnumerable<MiniProfileDto>>(_json));

		Assert.Contains(actual, dto => dto.Id == P5.GetId());
	}

	[Trait("Group", "Follow")]
	[Fact(DisplayName = "Should send a follow request")]
	public async Task CanFollow()
	{
		var P7 = _profiles[7];
		var P8 = _profiles[8];
		var path = $"/lb/v1/profiles/{P7.GetId25()}/following/{P8.GetId25()}";
		var response = await _client.PutAsync(path, null);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var content = await response.Content.ReadAsStringAsync();
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(
			await response.Content.ReadFromJsonAsync<FollowerRelationDto>(_json));

		Assert.Equal(P7.GetId(), actual.Follower);
		Assert.Equal(P8.GetId(), actual.Follows);
		Assert.Equal(FollowState.Accepted, actual.State);
	}

	[Trait("Group", "Follow")]
	[Fact(DisplayName = "Should send unfollow")]
	public async Task CanUnfollow()
	{
		var P9 = _profiles[9];
		var P8 = _profiles[8];
		var path = $"/lb/v1/profiles/{P8.GetId25()}/following/{P9.GetId25()}";
		var response = await _client.DeleteAsync(path);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var content = await response.Content.ReadAsStringAsync();
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(
			await response.Content.ReadFromJsonAsync<FollowerRelationDto>(_json));

		Assert.Equal(FollowState.None, actual.State);
	}

	[Trait("Group", "Follow")]
	[Fact(DisplayName = "Should remove follower")]
	public async Task CanRemoveFollower()
	{
		var P9 = _profiles[9];
		var P8 = _profiles[8];
		var path = $"/lb/v1/profiles/{P8.GetId25()}/follower/{P9.GetId25()}";
		var response = await _client.DeleteAsync(path);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var content = await response.Content.ReadAsStringAsync();
		var actual = Assert.IsAssignableFrom<FollowerRelationDto>(
			await response.Content.ReadFromJsonAsync<FollowerRelationDto>(_json));

		Assert.Equal(FollowState.None, actual.State);
	}
}