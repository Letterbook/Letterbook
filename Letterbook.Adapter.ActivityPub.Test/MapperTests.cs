using System.Reflection;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Actor;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Conversion;
using AutoMapper;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

/// <summary>
/// Mapper tests are a little bit of a mess right now, but half the mappers will need to be rebuilt in the near future
/// anyway.
/// </summary>
public class MapperTests : IClassFixture<JsonLdSerializerFixture>
{
    private readonly ITestOutputHelper _output;
    private static IMapper AstMapper => new Mapper(Mappers.AstMapper.Default);
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    private IMapper _modelMapper;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    private FakeProfile _fakeProfile;
    private Models.Profile _profile;
    private readonly IJsonLdSerializer _serializer;

    private static string DataDir => Path.Join(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MapperTests(ITestOutputHelper output, JsonLdSerializerFixture serializerFixture)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _output = output;
        _serializer = serializerFixture.JsonLdSerializer;

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile("letterbook.example");
        _profile = _fakeProfile.Generate();
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

    [Fact(Skip = "Need ModelMapper")]
    public void MapProfileDefault()
    {
        var actual = _modelMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(_profile.Id.ToString(), actual.Id);
        Assert.Equal(_profile.Inbox.ToString(), actual.Inbox.Id);
        Assert.Equal(_profile.Outbox.ToString(), actual.Outbox.Id);
        Assert.Equal(_profile.Following.ToString(), actual.Following?.Id);
        Assert.Equal(_profile.Followers.ToString(), actual.Followers?.Id);
    }

    [Fact(Skip = "Need ModelMapper")]
    public void CanMapProfileDefaultSigningKey()
    {
        var expected = _profile.Keys.First().GetRsa().ExportSubjectPublicKeyInfoPem();

        var actual = _modelMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(actual?.PublicKey?.PublicKeyPem, expected);
        Assert.Equal(actual?.PublicKey?.Owner?.Value?.Id, _profile.Id.ToString());
        Assert.Equal(actual?.PublicKey?.Id, _profile.Keys.First().Id.ToString());
    }


    [Fact(Skip = "broken, pending new model types (ADR-07)")]
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
        var actual = AstMapper.Map<Models.Note>(dto);

        Assert.NotNull(actual);
        Assert.Equal("Some test content", actual.Content);
        Assert.Equal("https://letterbook.example/@testuser", actual.Creators.First().Id.ToString());
    }

    [Fact(Skip = "Need ModelMapper")]
    public void CanMapActorCore()
    {
        var actual = _modelMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(_profile.Id.ToString(), actual.Id);
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
        var actual = _modelMapper.Map<PersonActorExtension>(_profile);

        Assert.Equal(expectedPem, actual.PublicKey?.PublicKeyPem);
        Assert.Equal(expectedKey.Id.ToString(), actual.PublicKey?.Id);
    }
}