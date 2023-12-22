using ActivityPub.Types.Conversion;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Letterbook.Core.Workers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class DeliveryWorkerTests : WithMocks, IClassFixture<JsonLdSerializerFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly DeliveryWorker _worker;
    private readonly Profile _profile;
    private readonly Profile _targetProfile;
    private readonly CloudEvent _event;

    public DeliveryWorkerTests(ITestOutputHelper output, JsonLdSerializerFixture serializer)
    {
        _output = output;
        _output.WriteLine($"Bogus seed: {Init.WithSeed()}");
        _worker = new DeliveryWorker(Mock.Of<ILogger<DeliveryWorker>>(), AccountProfileMock.Object,
            ActivityPubClientMock.Object);
        var faker = new FakeProfile("letterbook.example");
        _profile = faker.Generate();
        _targetProfile = new FakeProfile().Generate();
        _event =  new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Data = $$"""{"type": "Test", "attributedTo": "{{_profile.Id}}"}""",
            Type = "TestActivity",
            Subject = "TestActivity",
            Time = DateTimeOffset.UtcNow,
            [IActivityMessageService.DestinationKey] = _targetProfile.Inbox.ToString(),
            [IActivityMessageService.ProfileKey] =  _profile.LocalId!.Value.ToShortId(),
        };
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_worker);
    }

    [Fact(DisplayName = "Should send the AP document")]
    public async Task ShouldSend()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.IsAny<Guid>())).ReturnsAsync(_profile);

        await _worker.DoWork(_event);
        
        ActivityPubAuthClientMock.Verify(m => m.SendDocument(It.IsAny<Uri>(), It.Is<string>(s => s.Contains(_profile.Id.ToString()))));
    }
}