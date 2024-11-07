using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Collection;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Conversion;
using ActivityPub.Types.Util;
using AutoMapper;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Medo;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

/// <summary>
/// Mapper tests are a little bit of a mess right now, but half the mappers will need to be rebuilt in the near future
/// anyway.
/// </summary>
public class MapperTests : IClassFixture<JsonLdSerializerFixture>
{

	public class MapFromModelTests : IClassFixture<JsonLdSerializerFixture>
	{
		private readonly ITestOutputHelper _output;

		private FakeProfile _fakeProfile;
		private Models.Profile _profile;
		private readonly IJsonLdSerializer _serializer;
		private readonly FakePost _fakePost;
		private readonly Models.Post _post;
		private static IMapper _profileMapper = new Mapper(Mappers.AstMapper.Profile);
		private static IMapper _postMapper = new Mapper(Mappers.AstMapper.Post);

		public MapFromModelTests(ITestOutputHelper output, JsonLdSerializerFixture serializerFixture)
		{
			_output = output;
			_serializer = serializerFixture.JsonLdSerializer;

			_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
			_fakeProfile = new FakeProfile("letterbook.example");
			_profile = _fakeProfile.Generate();
			_fakePost = new FakePost(_profile);
			_post = _fakePost.Generate();
		}

		[Fact(DisplayName = "Map a simple Post to AS#Note")]
		public void MapSingleNotePost()
		{
			var actual = _postMapper.Map<NoteObject>(_post);

			Assert.NotNull(actual);
			Assert.Equal(_post.FediId.ToString(), actual.Id);
			Assert.Equal(_post.Creators.First().FediId.ToString(), actual.AttributedTo.First().Link?.HRef.ToString());
			Assert.Equal(_post.Contents.First().Html, actual.Content?.DefaultValue);
		}

		[Fact(DisplayName = "Map a profile to Actor")]
		public void MapProfileDefault()
		{
			var actual = _profileMapper.Map<ProfileActor>(_profile);

			Assert.Equal(_profile.FediId.ToString(), actual.Id);
			Assert.Equal(_profile.Inbox.ToString(), actual.Inbox);
			Assert.Equal(_profile.Outbox.ToString(), actual.Outbox);
			Assert.Equal(_profile.Following.ToString(), actual.Following?.HRef.ToString());
			Assert.Equal(_profile.Followers.ToString(), actual.Followers?.HRef.ToString());
			Assert.Equal(_profile.Handle, actual.PreferredUsername?.DefaultValue);
			Assert.Equal(_profile.DisplayName, actual.Name?.DefaultValue);
			Assert.True(actual.Is<ProfilePersonActor>());
		}

		[Fact(DisplayName = "Need ModelMapper")]
		public void CanMapActorExtensionsPublicKey()
		{
			var expectedKey = _profile.Keys.First();
			var expectedPem = expectedKey.GetRsa().ExportSubjectPublicKeyInfoPem();
			var actual = _profileMapper.Map<ProfileActor>(_profile);

			Assert.Equal(expectedPem, actual.PublicKeys.First().PublicKeyPem);
			Assert.Equal(expectedKey.FediId.ToString(), actual.PublicKeys.First().Id);
		}
	}

	public class MapFromAstTests : IClassFixture<JsonLdSerializerFixture>
	{
		private readonly ITestOutputHelper _output;
		private static IMapper AstMapper => new Mapper(Mappers.AstMapper.Default);
		private readonly IJsonLdSerializer _serializer;
		private NoteObject _simpleNote;

		public MapFromAstTests(ITestOutputHelper output, JsonLdSerializerFixture serializerFixture)
		{
			_output = output;
			_serializer = serializerFixture.JsonLdSerializer;

			_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");

			_simpleNote = new NoteObject
			{
				Id = "https://note.example/note/1",
				Content = "<p>test content</p>",
				Source = new ASObject
				{
					Content = "test content",
					MediaType = "text"
				}
			};
			_simpleNote.AttributedTo.Add("https://note.example/actor/1");
		}

		[Fact]
		public void ValidConfig()
		{
			Mappers.AstMapper.Default.AssertConfigurationIsValid();
			Mappers.AstMapper.Post.AssertConfigurationIsValid();
		}

		[Fact]
		public void CanMapLetterbookActor()
		{
			// TODO: Serialize a live profile
			using var fs = TestData.Read("LetterbookActor.json");
			var actor = _serializer.Deserialize<ProfileActor>(fs)!;
			var actual = AstMapper.Map<Models.Profile>(actor);

			Assert.NotNull(actual);
			Assert.Equal("http://localhost:3080/users/user/inbox", actual.Inbox.ToString());
			Assert.Equal("http://localhost:3080/users/user/outbox", actual.Outbox.ToString());
			Assert.NotEqual(new Models.ProfileId(Uuid7.Empty), actual.Id);

		}

		[Fact]
		public void CanMapMastodonActor()
		{
			using var fs = TestData.Read("Actor.json");
			var actor = _serializer.Deserialize<ProfileActor>(fs)!;
			var mapped = AstMapper.Map<Models.Profile>(actor);

			Assert.NotNull(mapped);
			Assert.Equal("user", mapped.Handle);
		}

		[Fact]
		public void CanMapSimpleNote()
		{
			var actual = AstMapper.Map<Models.Post>(_simpleNote);

			Assert.NotEqual(actual.Id, Guid.Empty);
			Assert.Single(actual.Contents);
			Assert.All(actual.Contents, content => Assert.Equal(actual.Id, content.Post.Id));
			Assert.Equal(actual.Id, actual.Contents.First().Post.Id);
			Assert.Equal(actual.ContentRootIdUri, actual.Contents.First().FediId);
		}

		[Fact]
		public void CanMapThreadFromContext()
		{
			var expected = "https://note.example/note/1/thread/";
			_simpleNote.Context = new Linkable<ASObject>(new ASLink() { HRef = expected });
			var actual = AstMapper.Map<Models.Post>(_simpleNote);

			Assert.Equal(expected, actual.Thread.FediId.ToString());
		}

		[Fact]
		public void CanMapThreadFromReplies()
		{
			var expected = "https://note.example/note/1/thread/";
			_simpleNote.Replies = new ASCollection { Id = expected };
			var actual = AstMapper.Map<Models.Post>(_simpleNote);

			Assert.Equal(expected, actual.Thread.FediId.ToString());
		}
		[Fact]
		public void CanMapThreadPreferContext()
		{
			var expected = "https://note.example/note/1/thread/";
			_simpleNote.Replies = new ASCollection { Id = expected };
			_simpleNote.Context = new Linkable<ASObject>(new ASLink() { HRef = "https://note.example/note/3" });
			var actual = AstMapper.Map<Models.Post>(_simpleNote);

			Assert.Equal(expected, actual.Thread.FediId.ToString());
		}

		[Fact]
		public void CanMapSigningKey()
		{
			using var fs = TestData.Read("Actor.json");
			var actor = _serializer.Deserialize<ProfileActor>(fs)!;
			var mapped = AstMapper.Map<Models.Profile>(actor);

			var key = mapped.Keys[0];
			// Reading an uninitialized Uuid7 causes an exception, this verifies
			// that it doesn't happen here
			_ = key.Id;

			Assert.Equal(TimeSpan.Zero, key.Created.Offset);

			Assert.NotNull(mapped);
		}

		[Fact]
		public void CanMapInstanceActor()
		{
			using var fs = TestData.Read("InstanceActor.json");
			var actor = _serializer.Deserialize<ASType>(fs)!;

			var mapped = AstMapper.Map<Models.IFederatedActor>(actor);

			Assert.NotNull(mapped);
		}

		[Fact]
		public void CanMapPersonActor()
		{
			using var fs = TestData.Read("Actor.json");
			var actor = _serializer.Deserialize<ASType>(fs)!;

			var mapped = AstMapper.Map<Models.IFederatedActor>(actor);

			Assert.NotNull(mapped);
		}
	}
}