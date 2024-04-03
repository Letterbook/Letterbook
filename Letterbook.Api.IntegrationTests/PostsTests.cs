using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Api.IntegrationTests.Fixtures;
using Letterbook.Api.Json;
using Letterbook.Api.Mappers;
using Letterbook.Api.Tests.Fakes;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
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
	private readonly Dictionary<Profile, List<Post>> _posts;
	private readonly ContentTextComparer _contentTextComparer = new ContentTextComparer();

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
		_posts = _host.Posts;
		_postDto = new FakePostDto(_profiles[0]);
		var postMappings = _host.Services.GetRequiredService<MappingConfigProvider>().Posts;
		_mapper = new Mapper(postMappings);
		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
	}

	private bool ContentComparer(ContentDto? arg1, ContentDto? arg2) => arg1 != null && arg2 != null && arg1.Type == arg2.Type && arg1.Summary == arg2.Summary;
	private bool TimestampMsComparer(DateTimeOffset? arg1, DateTimeOffset? arg2)
	{
		if (arg1 == null && arg2 == null) return true;
		if (arg1 == null || arg2 == null) return false;

		return Math.Abs((arg1.Value - arg2.Value).Milliseconds) == 0;
	}

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
	}

	[Fact(DisplayName = "Should publish a draft")]
	public async Task CanPublishDraft()
	{
		var profile = _profiles[2];
		var post = _posts[profile][1];
		var response = await _client.PostAsync($"/lb/v1/posts/{profile.GetId25()}/post/{post.GetId25()}", new StringContent(""));

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = Assert.IsType<PostDto>(await response.Content.ReadFromJsonAsync<PostDto>(_json));
		Assert.Equal(post.GetId(), body.Id);
		Assert.NotNull(body.PublishedDate);
	}

	[Fact(DisplayName = "Should update an existing post")]
	public async Task CanUpdatePosts()
	{
		var profile = _profiles[2];
		var post = _posts[profile][0];
		var dto = _mapper.Map<PostDto>(post);
		dto.Contents.First().Text = "This is the updated text";
		var payload = JsonContent.Create(dto, options: _json);
		var response = await _client.PutAsync($"/lb/v1/posts/{profile.GetId25()}/post/{post.GetId25()}", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<PostDto>(await response.Content.ReadFromJsonAsync<PostDto>(_json));
		Assert.Equal(post.GetId(), actual.Id);
		Assert.Equal("This is the updated text", actual.Contents.First().Text);
		Assert.Equal("This is the updated text", actual.Contents.First().Preview);
		Assert.Equal(post.CreatedDate, actual.CreatedDate, TimestampMsComparer);
		Assert.NotEqual(post.UpdatedDate, actual.UpdatedDate, TimestampMsComparer);
	}

	[Fact(DisplayName = "Should attach content to an existing post")]
	public async Task CanAttachContent()
	{
		var profile = _profiles[2];
		var post = _posts[profile][0];
		var dto = new ContentDto
		{
			SortKey = 1,
			Type = "Note",
			Text = "This is additional content",

		};
		var payload = JsonContent.Create(dto, options: _json);
		var response = await _client.PostAsync($"/lb/v1/posts/{profile.GetId25()}/post/{post.GetId25()}/content", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<PostDto>(await response.Content.ReadFromJsonAsync<PostDto>(_json));

		Assert.Equal(2, actual.Contents.Count);
		Assert.Contains(dto, actual.Contents, _contentTextComparer);
	}

	[Fact(DisplayName = "Should edit content in an existing post")]
	public async Task CanEditContent()
	{
		var profile = _profiles[1];
		var post = _posts[profile][0];
		var content = post.Contents.First();
		var dto = _mapper.Map<ContentDto>(content);
		dto.Summary = "This is the new summary";
		dto.Text = $"This is the updated text {Guid.NewGuid()}";
		var payload = JsonContent.Create(dto, options: _json);

		var response = await _client
			.PutAsync($"/lb/v1/posts/{profile.GetId25()}/post/{post.GetId25()}/content/{content.GetId25()}", payload);

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<PostDto>(await response.Content.ReadFromJsonAsync<PostDto>(_json));

		Assert.Equal(dto.Text, actual.Contents.First().Text);
		Assert.Equal(dto.Text, actual.Contents.First().Preview);
		Assert.Equal(dto.Summary, actual.Contents.First().Summary);
	}

	[Fact(DisplayName = "Should remove content in an existing post")]
	public async Task CanRemoveContent()
	{
		var profile = _profiles[1];
		var post = _posts[profile][1];
		var content = post.Contents.First();

		var response = await _client
			.DeleteAsync($"/lb/v1/posts/{profile.GetId25()}/post/{post.GetId25()}/content/{content.GetId25()}");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<PostDto>(await response.Content.ReadFromJsonAsync<PostDto>(_json));

		Assert.Empty(actual.Contents);
	}

	[Fact(DisplayName = "Should delete an existing post")]
	public async Task CanDeletePost()
	{
		var profile = _profiles[1];
		var post = _posts[profile][2];

		var response = await _client
			.DeleteAsync($"/lb/v1/posts/{profile.GetId25()}/post/{post.GetId25()}");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Theory(DisplayName = "Should lookup a post by Id")]
	[InlineData([true])]
	[InlineData([false])]
	public async Task CanGetPost(bool withThread)
	{
		var profile = _profiles[0];
		var post = _posts[profile][2];

		var response = await _client
			.GetAsync($"/lb/v1/posts/{profile.GetId25()}/post/{post.GetId25()}?withThread={withThread}");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<PostDto>(await response.Content.ReadFromJsonAsync<PostDto>(_json));

		Assert.Equal(post.GetId(), actual.Id);
		if (withThread)
			Assert.Equal(post.Thread.Posts.DistinctBy(p => p.Id).Count(), actual.Thread?.Posts.Count());
	}

	[Fact(DisplayName = "Should lookup a thread of posts by Id")]
	public async Task CanGetThread()
	{
		var profile = _profiles[0];
		var post = _posts[profile][2];

		var response = await _client
			.GetAsync($"/lb/v1/posts/{profile.GetId25()}/thread/{post.Thread.GetId25()}");

		Assert.NotNull(response);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert
			.IsAssignableFrom<IEnumerable<PostDto>>(await response.Content.ReadFromJsonAsync<IEnumerable<PostDto>>(_json));

		Assert.Equal(post.Thread.Posts.DistinctBy(p => p.Id).Count(), actual.Count());
	}

}

public class ContentTextComparer : IEqualityComparer<ContentDto>
{
	public bool Equals(ContentDto x, ContentDto y)
	{
		if (ReferenceEquals(x, null)) return false;
		if (ReferenceEquals(y, null)) return false;
		if (x.GetType() != y.GetType()) return false;
		return x.Summary == y.Summary && x.SortKey == y.SortKey && x.Type == y.Type && x.Text == y.Text;
	}

	public int GetHashCode(ContentDto obj)
	{
		return HashCode.Combine(obj.Summary, obj.SortKey, obj.Type, obj.Text);
	}
}