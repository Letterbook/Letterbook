using Letterbook.Adapter.ActivityPub;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Workers.Consumers;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Workers.Tests;

public class ActivityPublisherTests : WithMockBus<IActivityMessagePublisher, ActivityMessagePublisher>
{
	private readonly IActivityMessagePublisher _publisher;
	private readonly DeliveryWorker _consumer;
	private readonly Profile _actor;
	private readonly Profile _target;

	public ActivityPublisherTests(ITestOutputHelper output)
	{
		Services.AddScoped<IActivityPubDocument, Document>();

		_publisher = Provider.GetRequiredService<IActivityMessagePublisher>();
		_consumer = Provider.GetRequiredService<DeliveryWorker>();

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");

		_actor = new FakeProfile("letterbook.example").Generate();
		_target = new FakeProfile("peer.example").Generate();

		MockAuthorizeAllowAll();
	}

	protected override void ConfigureBus(IBusRegistrationConfigurator bus)
	{
		bus.AddConsumer<DeliveryWorker>();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
		Assert.NotNull(_consumer);
	}

	[Fact(DisplayName = "Should publish follow messages")]
	public async Task CanSendFollow()
	{
		await _publisher.Follow(_target.Inbox, _target, _actor);
		await Harness.Published.Any<ActivityMessage>();
		await Harness.Consumed.Any<ActivityMessage>();

		ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
	}

	[Fact(DisplayName = "Should publish unfollow messages")]
	public async Task CanSendUnfollow()
	{
		await _publisher.Unfollow(_target.Inbox, _target, _actor);
		await Harness.Published.Any<ActivityMessage>();
		await Harness.Consumed.Any<ActivityMessage>();

		ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
	}

	[Fact(DisplayName = "Should publish accept follower messages")]
	public async Task CanSendAccept()
	{
		await _publisher.AcceptFollower(_target.Inbox, _target, _actor);
		await Harness.Published.Any<ActivityMessage>();
		await Harness.Consumed.Any<ActivityMessage>();

		ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
	}

	[Fact(DisplayName = "Should publish reject follower messages")]
	public async Task CanSendReject()
	{
		await _publisher.RejectFollower(_target.Inbox, _target, _actor);
		await Harness.Published.Any<ActivityMessage>();
		await Harness.Consumed.Any<ActivityMessage>();

		ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
	}

	[Fact(DisplayName = "Should publish pending accept follower messages")]
	public async Task CanSendPending()
	{
		await _publisher.PendingFollower(_target.Inbox, _target, _actor);
		await Harness.Published.Any<ActivityMessage>();
		await Harness.Consumed.Any<ActivityMessage>();

		ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
	}

	[Fact(DisplayName = "Should publish remove follower messages")]
	public async Task CanSendRemove()
	{
		await _publisher.RemoveFollower(_target.Inbox, _target, _actor);
		await Harness.Published.Any<ActivityMessage>();
		await Harness.Consumed.Any<ActivityMessage>();

		ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
	}
}