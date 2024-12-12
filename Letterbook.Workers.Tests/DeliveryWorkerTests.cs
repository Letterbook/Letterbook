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
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Workers.Tests;

public sealed class DeliveryWorkerTests : WithMocks, IAsyncDisposable
{
	private readonly ServiceProvider _provider;
	private readonly IActivityScheduler _publisher;
	private readonly DeliveryWorker _consumer;
	private readonly ITestHarness _harness;
	private readonly Profile _profile;

	public DeliveryWorkerTests(ITestOutputHelper output)
	{
		var services = new ServiceCollection()
			.AddSingleton(output.BuildLoggerFactory(LogLevel.Debug))
			.AddMocks(this)
			.AddScoped<IActivityScheduler, ActivityScheduler>()
			.AddScoped<IActivityPubDocument, Document>()
			.AddMassTransitTestHarness(bus => { bus.AddConsumer<DeliveryWorker>(); });
		services.TryAddTypesModule();

		_provider = services.BuildServiceProvider();
		_publisher = _provider.GetRequiredService<IActivityScheduler>();
		_consumer = _provider.GetRequiredService<DeliveryWorker>();
		_harness = _provider.GetRequiredService<ITestHarness>();

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_profile = new FakeProfile("letterbook.example").Generate();

		MockAuthorizeAllowAll();
		_harness.Start().Wait();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
		Assert.NotNull(_consumer);
	}

	[Theory(DisplayName = "Should send ASDocs to their destination")]
	[MemberData(nameof(GenerateDoc), 50)]
	public async Task CanSend(ASObject asDoc, Uri inbox)
	{
		await _publisher.Deliver(inbox, asDoc, _profile);

		Assert.True(await _harness.Published.Any<ActivityMessage>());
		Assert.True(await _harness.Consumed.Any<ActivityMessage>());

		ActivityPubAuthClientMock.Verify(c => c.SendDocument(It.IsAny<Uri>(), It.IsAny<string>()));
		ActivityPubAuthClientMock.VerifyNoOtherCalls();
	}

	/*
	 * Support methods
	 */

	public static IEnumerable<object[]> GenerateDoc(int count)
	{
		var faker = new FakeProfile();
		foreach (var profile in faker.Generate(count))
		{
			yield return
			[
				Doc(profile.FediId),
				profile.FediId
			];
		}

		yield break;

		NoteObject Doc(Uri href)
		{
			var asDoc = new NoteObject();
			asDoc.AttributedTo.Add(new Linkable<ASObject>(new ASLink
			{
				HRef = href
			}));
			return asDoc;
		}
	}

	public async ValueTask DisposeAsync()
	{
		await _provider.DisposeAsync();
		await _harness.Stop();
	}
}