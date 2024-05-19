using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Util;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Tests;
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

public sealed class DeliveryWorkerTests : WithMocks, IAsyncDisposable
{
	private readonly ServiceProvider _provider;
	private readonly IActivityMessagePublisher _publisher;
	private readonly DeliveryWorker _consumer;
	private readonly ITestHarness _harness;
	private readonly Profile _profile;

	public DeliveryWorkerTests(ITestOutputHelper output)
	{
		_provider = new ServiceCollection()
			.AddMocks(this)
			.AddScoped<IActivityMessagePublisher, ActivityMessagePublisher>()
			.AddMassTransitTestHarness(bus =>
			{
				bus.AddConsumer<DeliveryWorker>();
			})
			.BuildServiceProvider();
		_publisher = _provider.GetRequiredService<IActivityMessagePublisher>();
		_consumer = _provider.GetRequiredService<DeliveryWorker>();
		_harness = _provider.GetRequiredService<ITestHarness>();

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_profile = new FakeProfile("letterbook.example").Generate();

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
		Assert.NotNull(_consumer);
	}

	[Fact(DisplayName = "Should consume ActivityMessages")]
	public async Task CanConsume()
	{
		await _harness.Start();
		var asDoc = new NoteObject();
		asDoc.AttributedTo.Add(new Linkable<ASObject>(new ASLink
		{
			HRef = _profile.FediId
		}));

		await _harness.Bus.Publish(new ActivityMessage
		{
			Subject = "test",
			Claims = [],
			Type = "Deliver",
			NextData = "fake message",
			OnBehalfOf = _profile.GetId(),
			Inbox = new Uri("https://peer.example/Actor/SharedInbox")
		});

		Assert.True(await _harness.Consumed.Any<ActivityMessage>());
	}

	[Fact(DisplayName = "Should publish ActivityMessages")]
	public async Task CanPublish()
	{
		await _harness.Start();
		var asDoc = new NoteObject();
		asDoc.AttributedTo.Add(new Linkable<ASObject>(new ASLink
		{
			HRef = _profile.FediId
		}));

		await _publisher.Deliver(new Uri("https://peer.example/Actor/SharedInbox"), asDoc, _profile);

		Assert.True(await _harness.Published.Any<ActivityMessage>());
	}

	[Fact(DisplayName = "Should send ASDocs to their destination")]
	public async Task CanSend()
	{
		await _harness.Start();
		var asDoc = new NoteObject();
		asDoc.AttributedTo.Add(new Linkable<ASObject>(new ASLink
		{
			HRef = _profile.FediId
		}));

		await _publisher.Deliver(new Uri("https://peer.example/Actor/SharedInbox"), asDoc, _profile);

		ActivityPubAuthClientMock.Verify(c => c.SendDocument(It.IsAny<Uri>(), It.IsAny<string>()));
		ActivityPubAuthClientMock.VerifyNoOtherCalls();
	}

	public async ValueTask DisposeAsync()
	{
		await _provider.DisposeAsync();
	}
}