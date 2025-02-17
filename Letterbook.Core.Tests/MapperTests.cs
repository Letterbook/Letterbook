using AutoMapper;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests.Fakes;
using Medo;

namespace Letterbook.Core.Tests;

public class MapperTests : WithMocks
{
	private PostDto _postDto = new();

	private MappingConfigProvider _mappingConfig;
	private readonly Mapper _postMapper;
	private readonly Mapper _profileMapper;
	private readonly UriBuilder _builder;

	public MapperTests()
	{
		_builder = new UriBuilder();
		_builder.Host = CoreOptionsMock.Value.DomainName;
		_mappingConfig = new MappingConfigProvider(CoreOptionsMock);
		_postMapper = new Mapper(_mappingConfig.Posts);
		_profileMapper = new Mapper(_mappingConfig.Profiles);
	}

	private Uri FediId(Uuid7 id)
	{
		_builder.Path = id.ToId25String();
		return _builder.Uri;
	}

	[Fact(DisplayName = "Posts config is valid")]
	public void ValidPosts()
	{
		_mappingConfig.Posts.AssertConfigurationIsValid();
	}

	[Fact(DisplayName = "Profiles config is valid")]
	public void ValidProfiles()
	{
		_mappingConfig.Profiles.AssertConfigurationIsValid();
	}

	[Fact(DisplayName = "Should round trip Post IDs")]
	public void MapRoundTrip()
	{
		var expected = new FakePost(CoreOptionsMock.Value.DomainName).Generate();
		var intermediate = _postMapper.Map<PostDto>(expected);
		var actual = _postMapper.Map<Post>(intermediate);

		Assert.Equal(expected.GetId(), actual.GetId());
		Assert.Equal(expected.FediId, actual.FediId);
		Assert.Equal(expected.Replies, actual.Replies);
		Assert.Equal(expected.Likes, actual.Likes);
		Assert.Equal(expected.Shares, actual.Shares);
	}

	[Fact(DisplayName = "Should map PostDto")]
	public void MapPost()
	{
		var actual = _postMapper.Map<Models.Post>(_postDto);

		Assert.NotNull(actual);
	}

	[InlineData("text/plain")]
	[InlineData("text/markdown")]
	[InlineData("text/html")]
	[InlineData(null)]
	[Theory(DisplayName = "Should map PostDto with content")]
	public void MapPostContent(string? type)
	{
		var noteDto = new ContentDto
		{
			Type = "Note",
			Text = "test text",
			SourceContentType = type,
		};
		_postDto.Contents.Add(noteDto);
		var actual = _postMapper.Map<Models.Post>(_postDto);

		Assert.NotNull(actual);
		var actualNote = Assert.IsType<Note>(actual.Contents.FirstOrDefault());
		Assert.Equal(type ?? "text/plain", actualNote.SourceContentType?.MediaType);
		Assert.Equal("text/html", actualNote.ContentType?.MediaType);
	}

	[InlineData("text/plain")]
	[InlineData("text/markdown")]
	[InlineData("text/html")]
	[InlineData(null)]
	[Theory(DisplayName = "Should map ContentDto as Note")]
	public void MapNote(string? type)
	{
		var noteDto = new ContentDto
		{
			Type = "Note",
			Text = "test text",
			SourceContentType = type,
		};
		var actual = _postMapper.Map<Models.Note>(noteDto);

		Assert.NotNull(actual);
		Assert.Equal(type ?? "text/plain", actual.SourceContentType?.ToString());
		Assert.Equal("text/html", actual.ContentType?.MediaType);
	}

	[Fact(DisplayName = "Should generate an Id")]
	public void MapPostNewId()
	{
		var actual = _postMapper.Map<Models.Post>(_postDto);

		Assert.NotEqual(Uuid7.Empty, actual.GetId());
	}

	[Fact(DisplayName = "Should map an Id")]
	public void MapPostId()
	{
		var expected = Uuid7.NewUuid7();
		_postDto.Id = expected;
		var actual = _postMapper.Map<Models.Post>(_postDto);

		Assert.Equal(expected, actual.GetId());
		// Assert.Equal();
	}

	[Fact(DisplayName = "Should map Creators")]
	public void MapPostCreators()
	{
		var uuid = Uuid7.NewUuid7();
		var expected = new MiniProfileDto
		{
			Id = uuid,
			FediId = FediId(uuid),
			DisplayName = "Test",
			Handle = "TestHandle"
		};

		_postDto.Creators = new List<MiniProfileDto> { expected };
		var mapped = _postMapper.Map<Models.Post>(_postDto);

		var actual = mapped.Creators.FirstOrDefault();
		Assert.NotNull(actual);
		Assert.Equal(uuid, actual.GetId());
		Assert.Equal(expected.FediId, actual.FediId);
		Assert.Equal(expected.DisplayName, actual.DisplayName);
		Assert.Equal(expected.Handle, actual.Handle);
	}

	// Setting the Thread properly in the mapper is too tricky to do reliably
	// But it's easy everywhere else
	[Fact(DisplayName = "Should not map InReplyTo")]
	public void MapPostInReplyTo()
	{
		var uuid = Uuid7.NewUuid7();
		var expected = new PostDto()
		{
			Id = uuid,
			FediId = FediId(uuid)
		};
		_postDto.InReplyTo = expected;

		var actual = _postMapper.Map<Models.Post>(_postDto);

		Assert.Null(actual.InReplyTo);
	}
	// Either it's a reply and we need to look up the Thread, or it's a top level post and we can should use the one from the
	// constructor
	[Fact(DisplayName = "Should not map Thread")]
	public void MapPostThread()
	{
		var uuid = Uuid7.NewUuid7();
		var expected = new ThreadDto
		{
			Id = uuid,
			FediId = FediId(uuid)
		};
		_postDto.Thread = expected;

		var actual = _postMapper.Map<Models.Post>(_postDto);

		Assert.NotEqual(uuid, actual.Thread.Id);
	}

	[Fact(DisplayName = "Should map the Audience")]
	public void MapPostAudience()
	{
		var uuid = Uuid7.NewUuid7();
		var expected = new AudienceDto
		{
			FediId = FediId(uuid)
		};
		_postDto.Audience.Add(expected);

		var mapped = _postMapper.Map<Models.Post>(_postDto);
		var actual = mapped.Audience.FirstOrDefault();

		Assert.NotNull(actual);
		Assert.Equal(expected.FediId, actual.FediId);
	}

	[Fact(DisplayName = "Should map Mentions")]
	public void MapPostMentions()
	{
		var uuid = Uuid7.NewUuid7();
		var expected = new MentionDto
		{
			Mentioned = uuid.ToId25String(),
			Visibility = Models.MentionVisibility.To
		};

		_postDto.AddressedTo = new List<MentionDto> { expected };
		var mapped = _postMapper.Map<Models.Post>(_postDto);

		var actual = mapped.AddressedTo.FirstOrDefault();
		Assert.NotNull(actual);
		Assert.Equal(actual.Subject.GetId(), uuid);
		Assert.Equal(actual.Visibility, expected.Visibility);
	}

	[Fact(DisplayName = "Should map ProfileDto")]
	public void CanMapProfileDto()
	{
		var uuid = Uuid7.NewUuid7();
		var expected = new FullProfileDto
		{
			Id = uuid,
			FediId = new Uri($"https://letterbook.example/actor/{uuid.ToId25String()}"),
			Handle = "Handle",
			DisplayName = "DisplayName",
			Description = "Description",
			CustomFields = [],
			Type = Models.ActivityActorType.Person,
		};

		var actual = _profileMapper.Map<Models.Profile>(expected);

		Assert.NotNull(actual);
		Assert.Equal(expected.Id, actual.GetId());
		Assert.Equal(expected.FediId, actual.FediId);
		Assert.Equal(expected.Handle, actual.Handle);
		Assert.Equal(expected.DisplayName, actual.DisplayName);
		Assert.Equal(expected.Description, actual.Description);
		Assert.Equal(expected.Type, actual.Type);
	}

	[Fact(DisplayName = "Should map Profile")]
	public void CanMapProfile()
	{
		var expected = new FakeProfile("letterbook.example").Generate();

		var actual = _profileMapper.Map<FullProfileDto>(expected);

		Assert.NotNull(actual);
		Assert.Equal(expected.GetId(), actual.Id);
		Assert.Equal(expected.FediId, actual.FediId);
		Assert.Equal(expected.Handle, actual.Handle);
		Assert.Equal(expected.DisplayName, actual.DisplayName);
		Assert.Equal(expected.Description, actual.Description);
		Assert.Equal(expected.Type, actual.Type);
	}

	[Fact(DisplayName = "Should map SigningKeys")]
	public void CanMapPublicKey()
	{
		var expected = new FakeProfile("letterbook.example").Generate().Keys.First();

		var actual = _profileMapper.Map<PublicKeyDto>(expected);

		Assert.NotNull(actual);
		Assert.Equal(expected.FediId, actual.FediId);
		Assert.Equal(expected.Label, actual.Label);
		Assert.Equal(expected.GetRsa().ExportSubjectPublicKeyInfoPem(), actual.PublicKeyPem);
		Assert.Equal(expected.Family.ToString(), actual.Family);
	}
}