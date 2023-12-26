using System.Reflection;
using System.Text.Json;
using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Adapter.ActivityPub.Mappers;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Tests.Fakes;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

public class MapperTests
{
    private readonly ITestOutputHelper _output;
    private IMapper _profileMapper;
    private IMapper _asApMapper;
    private IMapper _asApActor;
    private IMapper _APSharpMapper;
    private FakeProfile _fakeProfile;
    private Models.Profile _profile;

    private static string DataDir => Path.Join(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");


    public MapperTests(ITestOutputHelper output)
    {
        _output = output;
        _profileMapper = new Mapper(ProfileMappers.DefaultProfile);
        _asApMapper = new Mapper(AsApMapper.Config);
        _APSharpMapper = new Mapper(AsApMapper.ActorConfig);
        // _asApActor = new Mapper(AsApMapper.DefaultActor);

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile("letterbook.example");
        _profile = _fakeProfile.Generate();
    }

    [Fact]
    public void ValidConfig()
    {
        ProfileMappers.DefaultProfile.AssertConfigurationIsValid();
        AsApMapper.Config.AssertConfigurationIsValid();
    }

    [Fact]
    public void MapProfileDefault()
    {
        var actual = _profileMapper.Map<AsAp.Actor>(_profile);

        Assert.Equal(_profile.Id, actual.Id);
        Assert.Equal(_profile.Inbox, actual.Inbox.Id);
        Assert.Equal(_profile.Outbox, actual.Outbox.Id);
        Assert.Equal(_profile.Following, actual.Following?.Id);
        Assert.Equal(_profile.Followers, actual.Followers?.Id);
    }

    [Fact]
    public void CanMapProfileDefaultSigningKey()
    {
        var expected = _profile.Keys.First().GetRsa().ExportSubjectPublicKeyInfoPem();

        var actual = _profileMapper.Map<AsAp.Actor>(_profile);

        Assert.Equal(actual?.PublicKey?.PublicKeyPem, expected);
        Assert.Equal(actual?.PublicKey?.Owner.Id, _profile.Id);
        Assert.Equal(actual?.PublicKey?.Id, _profile.Keys.First().Id);
    }


    [Fact]
    public void CanMapSimpleNote()
    {
        var dto = new AsAp.Object()
        {
            Id = "https://mastodon.example/note/1234",
            Type = "Note",
            Content = "Some test content",
        };
        dto.AttributedTo.Add(new AsAp.Link("https://letterbook.example/@testuser"));
        var actual = _asApMapper.Map<Models.Note>(dto);

        Assert.NotNull(actual);
        Assert.Equal("Some test content", actual.Content);
        Assert.Equal("https://letterbook.example/@testuser", actual.Creators.First().Id.ToString());
    }

    [Fact]
    public void CanMapActor()
    {
        using var fs = new FileStream(Path.Join(DataDir, "Actor.json"), FileMode.Open);
        var actor = JsonSerializer.Deserialize<AsAp.Actor>(fs, JsonOptions.ActivityPub)!;

        var profile = _asApMapper.Map<Models.Profile>(actor);

        Assert.Equal("http://localhost:3080/users/user", profile.Id.ToString());
        Assert.Equal("http://localhost:3080/users/user/inbox", profile.Inbox.ToString());
        Assert.Equal(Models.ActivityActorType.Person, profile.Type);
    }

    [Fact]
    public void CanMapActorPublicKey()
    {
        var expected =
            "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4Kkwp47KloIaR8m9RJpddiaheHzm3f3qGja6CqQnw/gHxmlkJA/bFQXYzRZA8olVjsoPmp4gHCHRMzxm6eYeuRu0k628n0KkgZ+FapQGAamR9uNtAoV8Fr3x1FMqiiyOXP07kdsF4NUDOI+DotTezAGXrJT9AFqW1J7uJjL9aqWBJkwUkc/bjg12LZmtcvVftbEhzoi3RX4Etc4z5tK9VgHo6mENkZ5Hd6DOnG0ORVcFehZVamKACB9A7q5ln9l/jkCnGpAVzrl4lbTN5bfJ7cyZKkeQ+XNU7edzS6W9Crlekpal2L+J32Rwk6khTYGgY/a9jrfX//tVPUpwSKLlCwIDAQAB";
        using var fs = new FileStream(Path.Join(DataDir, "Actor.json"), FileMode.Open);
        var actor = JsonSerializer.Deserialize<AsAp.Actor>(fs, JsonOptions.ActivityPub)!;

        var profile = _asApMapper.Map<Models.Profile>(actor);
        var actual = profile.Keys.FirstOrDefault();

        Assert.NotNull(actual);
        Assert.Equal(expected, Convert.ToBase64String(actual.PublicKey.ToArray()));
        Assert.Equal("http://localhost:3080/users/user#main-key", actual.Id.ToString());
    }

    [Fact]
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

    [Fact]
    public void CanMapActorExtensionsPublicKey()
    {
        var expectedKey = _profile.Keys.First();
        var expectedPem = expectedKey.GetRsa().ExportSubjectPublicKeyInfoPem();
        var actual = _APSharpMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(expectedPem, actual.PublicKey?.PublicKeyPem);
        Assert.Equal(expectedKey.Id.ToString(), actual.PublicKey?.Id);
    }
}