using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Tests.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.IntegrationTests;

[Trait("ActivityPub", "Client")]
[Trait("Composite", "ActivityPub")]
[Trait("Composite", "Mastodon")]
public class ActivityPubClientCompositeTests : IClassFixture<HostFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly HostFixture _hostFactory;
    private FakeProfile _fakeProfile;

    public ActivityPubClientCompositeTests(ITestOutputHelper output, HostFixture hostFactory)
    {
        _output = output;
        _hostFactory = hostFactory;

        _output.WriteLine($"Bogus seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile(_hostFactory.Server.BaseAddress.Authority);
    }
    
    [Fact]
    public void Exists()
    {
    }

    [Fact(Skip = "no backend")]
    public async Task SendFollow()
    {
        var remote = new Uri("http://localhost:8200/anything");
        var profile = _fakeProfile.Generate();
        var target = new FakeProfile(remote).Generate();
        HostFixture.Mocks.AccountProfileMock.Setup(m => m.LookupProfile((Guid)profile.LocalId!))
            .ReturnsAsync(profile);
        HostFixture.Mocks.AccountProfileMock.Setup(m => m.LookupProfile(remote))
            .ReturnsAsync(target);

        using var scope = _hostFactory.Services.CreateScope();
        
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        var result = await profileService.Follow((Guid)profile.LocalId!, remote);
    }
}