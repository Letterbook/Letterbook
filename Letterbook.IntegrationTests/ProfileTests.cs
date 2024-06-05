using System.Net;
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
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
	private readonly HttpClient _client;
	private readonly List<Profile> _profiles;
	private readonly Dictionary<Profile, List<Post>> _posts;
	private readonly FakePostDto _postDto;
	private readonly Mapper _mapper;
	private readonly JsonSerializerOptions _json;
	private readonly List<Account> _accounts;
	private readonly IServiceScope _scope;
	static int? ITestSeed.Seed() => null;

	public ProfileTests(HostFixture<ProfileTests> host)
	{
		_host = host;
		_scope = host.CreateScope();
		_client = _host.Options == null
			? _host.CreateClient()
			: _host.CreateClient(new WebApplicationFactoryClientOptions()
			{
				BaseAddress = _host.Options.BaseUri()
			});
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
}