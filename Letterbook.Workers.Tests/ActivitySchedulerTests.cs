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

public class Activities : WithMockBus<IActivityScheduler, ActivityScheduler>
{
	private readonly IActivityScheduler _publisher;
	private readonly DeliveryWorker _consumer;
	private readonly Profile _actor;
	private readonly Profile _target;

	public Activities(ITestOutputHelper output)
	{
		UseProjectRelativeDirectory("../Snapshots");

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

	public class Follow(ITestOutputHelper output) : Activities(output)
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

	public class Unfollow(ITestOutputHelper output) : Activities(output)
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

	public class AcceptFollower(ITestOutputHelper output) : Activities(output)
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

	public class RejectFollower(ITestOutputHelper output) : Activities(output)
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

	public class PendingFollower(ITestOutputHelper output) : Activities(output)
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

	public class RemoveFollower(ITestOutputHelper output) : Activities(output)
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

	public class Report(ITestOutputHelper output) : Activities(output)
	{
		[Fact(DisplayName = "Should match the snapshot with a full context report")]
		public async Task ShouldMatch_FullContext()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate(3);
			var other = new FakeProfile("different.example").UseSeed(4).Generate();
			var report = new FakeReport(actor, target[0]).UseSeed(2).Generate();
			var posts = new FakePost(target[0]).UseSeed(1).Generate(3);
			var otherPost = new FakePost(other).UseSeed(4).Generate();
			report.Subjects.Add(target[1]);
			report.Subjects.Add(target[2]);
			report.Subjects.Add(other);
			foreach (var post in posts)
			{
				report.RelatedPosts.Add(post);
			}
			report.RelatedPosts.Add(otherPost);
			await _publisher.Report(target[0].Inbox, report, true);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should match the snapshot with a minimal report")]
		public async Task ShouldMatch()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			var report = new FakeReport(actor, target).UseSeed(2).Generate();
			await _publisher.Report(target.Inbox, report, false);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should match the snapshot with a post")]
		public async Task ShouldMatch_WithPost()
		{
			var actor = new FakeProfile("letterbook.example").UseSeed(0).Generate();
			var target = new FakeProfile("peer.example").UseSeed(1).Generate();
			var post = new FakePost(target).UseSeed(1).Generate();
			var report = new FakeReport(actor, post).UseSeed(2).Generate();
			await _publisher.Report(target.Inbox, report, false);
			var actual = (await Harness.Published.SelectAsync<ActivityMessage>().FirstOrDefault())?.Context.Message.Data;

			Assert.NotNull(actual);
			await Verify(actual);
		}

		[Fact(DisplayName = "Should publish report messages")]
		public async Task CanSendReport()
		{
			var report = new FakeReport(_actor, _target).Generate();

			await _publisher.Report(_target.Inbox, report);
			await Harness.Published.Any<ActivityMessage>();
			await Harness.Consumed.Any<ActivityMessage>();

			ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()));
		}

		[Fact(DisplayName = "Should publish report messages for multiple profiles")]
		public async Task CanSendReport_Multiple()
		{
			var report = new FakeReport(_actor, _target).Generate();
			report.Subjects.Add(new FakeProfile("peer.example").Generate());

			await _publisher.Report(_target.Inbox, report);
			await Harness.Published.Any<ActivityMessage>();
			await Harness.Consumed.SelectAsync<ActivityMessage>().Skip(1).FirstOrDefaultAsync();

			ActivityPubAuthClientMock.Verify(m => m.SendDocument(_target.Inbox, It.IsAny<string>()), Times.Exactly(2));
		}
	}
}