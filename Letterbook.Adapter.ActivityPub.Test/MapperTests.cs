using System.Reflection;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Collection;
using ActivityPub.Types.AS.Extended.Actor;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Conversion;
using ActivityPub.Types.Util;
using AutoMapper;
using Letterbook.Adapter.ActivityPub.Types;
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
    private static string DataDir => Path.Join(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");

    public class MapFromModelTests : IClassFixture<JsonLdSerializerFixture>
    {
        private readonly ITestOutputHelper _output;

        private FakeProfile _fakeProfile;
        private Models.Profile _profile;
        private readonly IJsonLdSerializer _serializer;
#pragma warning disable CS8618
#pragma warning disable CS0649
        private static IMapper ModelMapper;
#pragma warning restore CS0649
#pragma warning restore CS8618

        public MapFromModelTests(ITestOutputHelper output, JsonLdSerializerFixture serializerFixture)
        {
            _output = output;
            _serializer = serializerFixture.JsonLdSerializer;

            _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
            _fakeProfile = new FakeProfile("letterbook.example");
            _profile = _fakeProfile.Generate();
        }

        [Fact(Skip = "Need ModelMapper")]
        public void MapProfileDefault()
        {
            var actual = ModelMapper.Map<PersonActorExtension>(_profile);

            Assert.Equal(_profile.FediId.ToString(), actual.Id);
            Assert.Equal(_profile.Inbox.ToString(), actual.Inbox.Id);
            Assert.Equal(_profile.Outbox.ToString(), actual.Outbox.Id);
            Assert.Equal(_profile.Following.ToString(), actual.Following?.Id);
            Assert.Equal(_profile.Followers.ToString(), actual.Followers?.Id);
        }

        [Fact(Skip = "Need ModelMapper")]
        public void CanMapProfileDefaultSigningKey()
        {
            var expected = _profile.Keys.First().GetRsa().ExportSubjectPublicKeyInfoPem();

            var actual = ModelMapper.Map<PersonActorExtension>(_profile);

            Assert.Equal(actual?.PublicKey?.PublicKeyPem, expected);
            Assert.Equal(actual?.PublicKey?.Owner?.Value?.Id, _profile.FediId.ToString());
            Assert.Equal(actual?.PublicKey?.Id, _profile.Keys.First().FediId.ToString());
        }

        [Fact(Skip = "Need ModelMapper")]
        public void CanMapActorCore()
        {
            var actual = ModelMapper.Map<PersonActorExtension>(_profile);

            Assert.Equal(_profile.FediId.ToString(), actual.Id);
            Assert.Equal(_profile.Inbox.ToString(), actual.Inbox.HRef);
            Assert.Equal(_profile.Outbox.ToString(), actual.Outbox.HRef);
            Assert.Equal(_profile.Following.ToString(), actual.Following?.HRef!);
            Assert.Equal(_profile.Followers.ToString(), actual.Followers?.HRef!);
            Assert.Equal(_profile.Handle, actual.PreferredUsername?.DefaultValue);
            Assert.Equal(_profile.DisplayName, actual.Name?.DefaultValue);
        }

        [Fact(Skip = "Need ModelMapper")]
        public void CanMapActorExtensionsPublicKey()
        {
            var expectedKey = _profile.Keys.First();
            var expectedPem = expectedKey.GetRsa().ExportSubjectPublicKeyInfoPem();
            var actual = ModelMapper.Map<PersonActorExtension>(_profile);

            Assert.Equal(expectedPem, actual.PublicKey?.PublicKeyPem);
            Assert.Equal(expectedKey.FediId.ToString(), actual.PublicKey?.Id);
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
        }

        [Fact]
        public void CanMapLetterbookActor()
        {
            using var fs = new FileStream(Path.Join(DataDir, "LetterbookActor.json"), FileMode.Open);
            var actor = _serializer.Deserialize<PersonActorExtension>(fs)!;
            var mapped = AstMapper.Map<Models.Profile>(actor);

            Assert.NotNull(mapped);
        }

        [Fact]
        public void CanMapMastodonActor()
        {
            using var fs = new FileStream(Path.Join(DataDir, "Actor.json"), FileMode.Open);
            var actor = _serializer.Deserialize<PersonActorExtension>(fs)!;
            var mapped = AstMapper.Map<Models.Profile>(actor);

            Assert.NotNull(mapped);
        }

        [Fact]
        public void CanMapSimpleNote()
        {
            var actual = AstMapper.Map<Models.Post>(_simpleNote);

            Assert.NotEqual(actual.Id, Uuid7.Empty);
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
    }
}