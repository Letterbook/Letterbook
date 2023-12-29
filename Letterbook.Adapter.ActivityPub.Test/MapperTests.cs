using System.Reflection;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Actor;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Conversion;
using AutoMapper;
using Letterbook.Adapter.ActivityPub.Mappers;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

public class MapperTests : IClassFixture<JsonLdSerializerFixture>
{
    private readonly ITestOutputHelper _output;
    private IMapper _profileMapper;
    private IMapper _astMapper;
    private IMapper _modelMapper;
    private IMapper _APSharpMapper;
    private FakeProfile _fakeProfile;
    private Models.Profile _profile;
    private readonly IJsonLdSerializer _serializer;

    private static string DataDir => Path.Join(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");


    public MapperTests(ITestOutputHelper output, JsonLdSerializerFixture serializerFixture)
    {
        _output = output;
        _serializer = serializerFixture.JsonLdSerializer;
        _astMapper = new Mapper(AstMapper.Default);
        // _modelMapper = new Mapper(ModelMappers.Profile);

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile("letterbook.example");
        _profile = _fakeProfile.Generate();
    }

    [Fact]
    public void ValidConfig()
    {
        AstMapper.Default.AssertConfigurationIsValid();
    }

    [Fact]
    public void CanMapActor2()
    {
        using var fs = new FileStream(Path.Join(DataDir, "Actor.json"), FileMode.Open);
        var actor = _serializer.Deserialize<PersonActorExtension>(fs)!;
        var mapped = _astMapper.Map<Profile>(actor);
        
        Assert.NotNull(mapped);
    }

    [Fact(Skip = "broken")]
    public void MapProfileDefault()
    {
        var actual = _profileMapper.Map<PersonActor>(_profile);

        Assert.Equal(_profile.Id.ToString(), actual.Id);
        Assert.Equal(_profile.Inbox.ToString(), actual.Inbox.Id);
        Assert.Equal(_profile.Outbox.ToString(), actual.Outbox.Id);
        Assert.Equal(_profile.Following.ToString(), actual.Following?.Id);
        Assert.Equal(_profile.Followers.ToString(), actual.Followers?.Id);
    }

    [Fact(Skip = "broken")]
    public void CanMapProfileDefaultSigningKey()
    {
        var expected = _profile.Keys.First().GetRsa().ExportSubjectPublicKeyInfoPem();

        var actual = _profileMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(actual?.PublicKey?.PublicKeyPem, expected);
        Assert.Equal(actual?.PublicKey?.Owner?.Value?.Id, _profile.Id.ToString());
        Assert.Equal(actual?.PublicKey?.Id, _profile.Keys.First().Id.ToString());
    }


    [Fact(Skip = "broken")]
    public void CanMapSimpleNote()
    {
        var dto = new NoteObject()
        {
            Id = "https://mastodon.example/note/1234",
            Content = "Some test content",
        };
        dto.AttributedTo.Add(new ASLink
        {
            HRef = "https://letterbook.example/@testuser"
        });
        var actual = _astMapper.Map<Models.Note>(dto);

        Assert.NotNull(actual);
        Assert.Equal("Some test content", actual.Content);
        Assert.Equal("https://letterbook.example/@testuser", actual.Creators.First().Id.ToString());
    }

    [Fact(Skip = "broken")]
    public void CanMapActor()
    {
        using var fs = new FileStream(Path.Join(DataDir, "Actor.json"), FileMode.Open);
        var actor = _serializer.Deserialize<PersonActorExtension>(fs)!;

        var profile = _astMapper.Map<Models.Profile>(actor);

        Assert.Equal("http://localhost:3080/users/user", profile.Id.ToString());
        Assert.Equal("http://localhost:3080/users/user/inbox", profile.Inbox.ToString());
        Assert.Equal(Models.ActivityActorType.Person, profile.Type);
    }

    [Fact(Skip = "broken")]
    public void CanMapActorPublicKey()
    {
        var expected =
            "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4Kkwp47KloIaR8m9RJpddiaheHzm3f3qGja6CqQnw/gHxmlkJA/bFQXYzRZA8olVjsoPmp4gHCHRMzxm6eYeuRu0k628n0KkgZ+FapQGAamR9uNtAoV8Fr3x1FMqiiyOXP07kdsF4NUDOI+DotTezAGXrJT9AFqW1J7uJjL9aqWBJkwUkc/bjg12LZmtcvVftbEhzoi3RX4Etc4z5tK9VgHo6mENkZ5Hd6DOnG0ORVcFehZVamKACB9A7q5ln9l/jkCnGpAVzrl4lbTN5bfJ7cyZKkeQ+XNU7edzS6W9Crlekpal2L+J32Rwk6khTYGgY/a9jrfX//tVPUpwSKLlCwIDAQAB";
        using var fs = new FileStream(Path.Join(DataDir, "Actor.json"), FileMode.Open);
        var actor = _serializer.Deserialize<PersonActorExtension>(fs)!;

        var profile = _astMapper.Map<Models.Profile>(actor);
        var actual = profile.Keys.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, Convert.ToBase64String(actual.PublicKey.ToArray()));
        Assert.Equal("http://localhost:3080/users/user#main-key", actual.Id.ToString());
    }

    [Fact(Skip = "broken")]
    public void CanMapActorCore()
    {
        var actual = _APSharpMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(_profile.Id.ToString(), actual.Id);
        Assert.Equal(_profile.Inbox.ToString(), actual.Inbox.HRef);
        Assert.Equal(_profile.Outbox.ToString(), actual.Outbox.HRef);
        Assert.Equal(_profile.Following.ToString(), actual.Following?.HRef!);
        Assert.Equal(_profile.Followers.ToString(), actual.Followers?.HRef!);
        Assert.Equal(_profile.Handle, actual.PreferredUsername?.DefaultValue);
        Assert.Equal(_profile.DisplayName, actual.Name?.DefaultValue);
    }

    [Fact(Skip = "broken")]
    public void CanMapActorExtensionsPublicKey()
    {
        var expectedKey = _profile.Keys.First();
        var expectedPem = expectedKey.GetRsa().ExportSubjectPublicKeyInfoPem();
        var actual = _APSharpMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(expectedPem, actual.PublicKey?.PublicKeyPem);
        Assert.Equal(expectedKey.Id.ToString(), actual.PublicKey?.Id);
    }
}