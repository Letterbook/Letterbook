using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.IntegrationTests.Fixtures;
using Letterbook.Api.Json;
using Letterbook.Api.Mappers;
using Letterbook.Api.Tests.Fakes;
using Letterbook.Core.Extensions;
using Medo;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.Api.IntegrationTests;

[Collection("Integration")]
public class PostsTests : IClassFixture<HostFixture>
{
	private readonly HostFixture _host;
	private readonly HttpClient _client;
	private readonly List<Profile> _profiles;
	private readonly FakePostDto _postDto;
	private readonly Mapper _mapper;
	private readonly JsonSerializerOptions _json;


	public PostsTests(HostFixture host)
	{
		_host = host;
		_client = _host.Options == null
			? _host.CreateClient()
			: _host.CreateClient(new WebApplicationFactoryClientOptions()
			{
				BaseAddress = _host.Options.BaseUri()
			});
		_profiles = _host.Profiles;
		_postDto = new FakePostDto(_profiles[0]);
		var postMappings = _host.Services.GetRequiredService<MappingConfigProvider>().Posts;
		_mapper = new Mapper(postMappings);
		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() }
		};
	}

	private bool ContentComparer(ContentDto? arg1, ContentDto? arg2) => arg1 != null && arg2 != null && arg1.Type == arg2.Type && arg1.Summary == arg2.Summary;

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
	}

	[Fact(DisplayName = "Should accept a draft post for a note")]
	public async Task CanDraftNote()
	{
		var profile = _profiles[0];
		var dto = _postDto.Generate();
		var payload = JsonContent.Create(dto, options: _json);
		var response = await _client.PostAsync($"/lb/v1/posts/{profile.GetId25()}/post", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = Assert.IsType<PostDto>(await response.Content.ReadFromJsonAsync<PostDto>(_json));
		Assert.NotNull(body.Id);
		Assert.NotEqual(Uuid7.Empty, body.Id);
		Assert.Equal(dto.Contents.First(), body.Contents.FirstOrDefault(), ContentComparer);
		// Assert.True(response.IsSuccessStatusCode);
	}

}