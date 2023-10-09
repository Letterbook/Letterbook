using AutoMapper;
using Letterbook.Adapter.ActivityPub.Mappers;
using Letterbook.Core.Tests.Fakes;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

public class MapperTests
{
    private readonly ITestOutputHelper _output;
    private IMapper _mapper;
    private FakeProfile _fakeProfile;
    private Models.Profile _profile;

    public MapperTests(ITestOutputHelper output)
    {
        _output = output;
        _mapper = new Mapper(ProfileMappers.DefaultProfile);
        
        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile("letterbook.example");
        _profile = _fakeProfile.Generate();
    }

    [Fact]
    [Trait("Mappers", "Profile")]
    public void ValidConfig()
    {
        ProfileMappers.DefaultProfile.AssertConfigurationIsValid();
    }

    [Fact]
    [Trait("Mappers", "Profile")]
    public void MapProfileDefault()
    {
        var actual = _mapper.Map<AsAp.Actor>(_profile);
        
        Assert.Equal(actual.Id, _profile.Id);
        Assert.Equal(actual.Inbox.Id, _profile.Inbox);
        Assert.Equal(actual.Outbox.Id, _profile.Outbox);
        Assert.Equal(actual.Following?.Id, _profile.Following.Id);
        Assert.Equal(actual.Followers?.Id, _profile.Followers.Id);
    }
}