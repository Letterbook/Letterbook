using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Workers.Consumers;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Workers.Tests;

public class ActivitySchedulerTests : WithMockBus<IActivityScheduler, ActivityScheduler>
{
	private readonly IActivityScheduler _publisher;
	private readonly DeliveryWorker _consumer;
	private readonly Profile _actor;
	private readonly Profile _target;

	public ActivitySchedulerTests(ITestOutputHelper output)
	{
		UseProjectRelativeDirectory("Snapshots");

		_publisher = Provider.GetRequiredService<IActivityScheduler>();
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

	public class Follow(ITestOutputHelper output) : ActivitySchedulerTests(output)
	{
		[Fact(DisplayName = "Should match the snapshot")]
		public async Task ShouldMatch()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			await _publisher.Follow(target.Inbox, target, actor);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should publish follow messages")]
		public async Task CanSendFollow()
		{
			await _publisher.Follow(_target.Inbox, _target, _actor);
			await Harness.Published.Any<ActivityMessage>();
			await Harness.Consumed.Any<ActivityMessage>();

			ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
		}
	}

	public class Unfollow(ITestOutputHelper output) : ActivitySchedulerTests(output)
	{
		[Fact(DisplayName = "Should match the snapshot")]
		public async Task ShouldMatch()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			await _publisher.Unfollow(target.Inbox, target, actor);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should publish unfollow messages")]
		public async Task CanSendUnfollow()
		{
			await _publisher.Unfollow(_target.Inbox, _target, _actor);
			await Harness.Published.Any<ActivityMessage>();
			await Harness.Consumed.Any<ActivityMessage>();

			ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
		}
	}

	public class AcceptFollower(ITestOutputHelper output) : ActivitySchedulerTests(output)
	{
		[Fact(DisplayName = "Should match the snapshot")]
		public async Task ShouldMatch()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			await _publisher.AcceptFollower(target.Inbox, target, actor);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should publish accept follower messages")]
		public async Task CanSendAccept()
		{
			await _publisher.AcceptFollower(_target.Inbox, _target, _actor);
			await Harness.Published.Any<ActivityMessage>();
			await Harness.Consumed.Any<ActivityMessage>();

			ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
		}
	}

	public class RejectFollower(ITestOutputHelper output) : ActivitySchedulerTests(output)
	{
		[Fact(DisplayName = "Should match the snapshot")]
		public async Task ShouldMatch()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			await _publisher.RejectFollower(target.Inbox, target, actor);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should publish reject follower messages")]
		public async Task CanSendReject()
		{
			await _publisher.RejectFollower(_target.Inbox, _target, _actor);
			await Harness.Published.Any<ActivityMessage>();
			await Harness.Consumed.Any<ActivityMessage>();

			ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
		}
	}

	public class PendingFollower(ITestOutputHelper output) : ActivitySchedulerTests(output)
	{
		[Fact(DisplayName = "Should match the snapshot")]
		public async Task ShouldMatch()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			await _publisher.PendingFollower(target.Inbox, target, actor);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should publish pending accept follower messages")]
		public async Task CanSendPending()
		{
			await _publisher.PendingFollower(_target.Inbox, _target, _actor);
			await Harness.Published.Any<ActivityMessage>();
			await Harness.Consumed.Any<ActivityMessage>();

			ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
		}
	}

	public class RemoveFollower(ITestOutputHelper output) : ActivitySchedulerTests(output)
	{
		[Fact(DisplayName = "Should match the snapshot")]
		public async Task ShouldMatch()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			await _publisher.RemoveFollower(target.Inbox, target, actor);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
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
}