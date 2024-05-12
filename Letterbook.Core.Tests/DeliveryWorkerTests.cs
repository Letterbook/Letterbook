using CloudNative.CloudEvents;
using Divergic.Logging.Xunit;
using Letterbook.Core.Events;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Letterbook.Core.Workers;
using Medo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Core.Tests;

public class DeliveryWorkerTests : WithMocks, IClassFixture<JsonLdSerializerFixture>
{
	private readonly ITestOutputHelper _output;
	private readonly Mock<DeliveryWorker> _mockWorker;
	private readonly DeliveryWorker _worker;
	private readonly Profile _profile;
	private readonly Profile _targetProfile;
	private readonly CloudEvent _event;

	public DeliveryWorkerTests(ITestOutputHelper output, JsonLdSerializerFixture serializer)
	{
		_output = output;
		_output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_mockWorker = new Mock<DeliveryWorker>(output.BuildLoggerFor<DeliveryWorker>(),
			AccountProfileMock.Object, ActivityPubClientMock.Object);
		_mockWorker.CallBase = true;
		_worker = _mockWorker.Object;

		MockedServiceCollection.AddScoped<DeliveryWorker>(_ => _mockWorker.Object);

		var faker = new FakeProfile("letterbook.example");
		_profile = faker.Generate();
		_targetProfile = new FakeProfile().Generate();
		_event = new CloudEvent
		{
			Id = Guid.NewGuid().ToString(),
			Data = $$"""{"type": "Test", "attributedTo": "{{_profile.FediId}}"}""",
			Type = "TestActivity",
			Subject = "TestActivity",
			Time = DateTimeOffset.UtcNow,
			[IActivityMessage.DestinationKey] = _targetProfile.Inbox.ToString(),
			[IActivityMessage.ProfileKey] = _profile.GetId25(),
		};
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_worker);
	}

	[Fact(DisplayName = "Should send the AP document", Skip = "MassTransit")]
	public void ShouldSend()
	{
		// TODO: the test

		ActivityPubAuthClientMock.Verify(m =>
			m.SendDocument(It.IsAny<Uri>(), It.Is<string>(s => s.Contains(_profile.FediId.ToString()))));
	}
}