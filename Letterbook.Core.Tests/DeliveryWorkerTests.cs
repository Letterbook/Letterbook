using ActivityPub.Types.Conversion;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Letterbook.Core.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class DeliveryWorkerTests : WithMocks, IClassFixture<JsonLdSerializerFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly Mock<DeliveryWorker> _mockWorker;
    private readonly Mock<ILogger<DeliveryObserver>> _observerLoggerMock;
    private readonly DeliveryWorker _worker;
    private readonly Profile _profile;
    private readonly Profile _targetProfile;
    private readonly CloudEvent _event;
    private readonly DeliveryObserver _observer;

    public DeliveryWorkerTests(ITestOutputHelper output, JsonLdSerializerFixture serializer)
    {
        _output = output;
        _output.WriteLine($"Bogus seed: {Init.WithSeed()}");
        _mockWorker = new Mock<DeliveryWorker>(Mock.Of<ILogger<DeliveryWorker>>(), AccountProfileMock.Object,
            ActivityPubClientMock.Object);
        _mockWorker.CallBase = true;
        _worker = _mockWorker.Object;
        _observerLoggerMock = new Mock<ILogger<DeliveryObserver>>();

        MockedServiceCollection.AddScoped<DeliveryWorker>(_ => _mockWorker.Object);
        _observer = new DeliveryObserver(_observerLoggerMock.Object, MockedServiceCollection.BuildServiceProvider());


        
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
        Assert.NotNull(_observer);
    }

    [Fact(DisplayName = "Should send the AP document")]
    public void ShouldSend()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.IsAny<Guid>())).ReturnsAsync(_profile);

        _observer.OnNext(_event);
        
        ActivityPubAuthClientMock.Verify(m => m.SendDocument(It.IsAny<Uri>(), It.Is<string>(s => s.Contains(_profile.Id.ToString()))));
    }
    
    [Fact(DisplayName = "Should warn if the channel closes")]
    public void ShouldLogComplete()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.IsAny<Guid>())).ReturnsAsync(_profile);

        _observer.OnCompleted();
        
        _observerLoggerMock.VerifyLog(m => m.LogWarning(It.IsAny<string?>(), It.IsAny<object?[]>()), Times.Once);
    }
    
    [Fact(DisplayName = "Should log error if the channel has an error")]
    public void ShouldLogError()
    {
        AccountProfileMock.Setup(m => m.LookupProfile(It.IsAny<Guid>())).ReturnsAsync(_profile);

        _observer.OnError(new Exception("Test exception"));
        
        _observerLoggerMock.VerifyLog(m => m.LogError(It.IsAny<Exception>(), It.IsAny<string?>(), It.IsAny<object?[]>()), Times.Once);
    }
}