using System.Text.Json.Serialization;
using Letterbook.Core.Models;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Workers.Consumers;
using Letterbook.Workers.Contracts;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Letterbook.Workers.Tests;

public sealed class TimelineConsumerTests : WithMocks, IAsyncDisposable
{
	private readonly Post _post;
	private readonly ITestHarness _harness;
	private readonly ServiceProvider _provider;
	private FaultObserver _faultObserver;

	public TimelineConsumerTests(ITestOutputHelper output)
	{
		_provider = new ServiceCollection()
			.AddSingleton<ILoggerFactory>(_ => output.BuildLoggerFactory(LogLevel.Error))
			.AddMocks(this)
			.AddMassTransitTestHarness(bus =>
			{
				bus.AddConsumer<TestConsumer>();
				bus.AddConsumer<TimelineConsumer>();
				bus.UsingInMemory((ctx, cfg) =>
				{
					cfg.UseDelayedMessageScheduler();
					cfg.ConfigureJsonSerializerOptions(options =>
					{
						options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
						// options.IncludeFields = true;

						return options;
					});
					cfg.ConfigureEndpoints(ctx);
				});
			})
			.BuildServiceProvider(true);
		_harness = _provider.GetRequiredService<ITestHarness>();
		_faultObserver = new FaultObserver();
		_harness.Bus.ConnectReceiveObserver(_faultObserver);

		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_post = new FakePost("letterbook.example").Generate();
		_harness.Start().Wait();
	}

	[Fact]
	public void Exists()
	{
		// Assert.NotNull(_consumer);
	}

	[Fact(DisplayName = "TestEvent")]
	public async Task CanConsumeTest()
	{
		await _harness.Bus.Publish<TestEvent>(new TestEvent
		{
			Data = nameof(CanConsumeCreate)
		});

		Assert.True(await _harness.Published.Any<TestEvent>());
		Assert.True(await _harness.Consumed.Any<TestEvent>());
		Assert.Empty(_faultObserver.Faults);
	}

	[Fact(DisplayName = "Should handle published Created events")]
	public async Task CanConsumeCreate()
	{
		_post.PublishedDate = DateTimeOffset.UtcNow;

		await _harness.Bus.Publish<PostEvent>(new PostEvent
		{
			Subject = _post.GetId25(),
			Claims = [],
			NextData = _post,
			Type = "Created",
		});

		Assert.True(await _harness.Published.Any<PostEvent>());
		Assert.True(await _harness.Consumed.Any<PostEvent>());
		// TimelineServiceMock.Verify(m => m.HandlePublish(It.IsAny<Post>()), Times.Once);
		TimelineServiceMock.VerifyNoOtherCalls();
	}

	public async ValueTask DisposeAsync()
	{
		await _provider.DisposeAsync();
		await _harness.Stop();
	}
}

public class TestConsumer : IConsumer<TestEvent>
{
	public Task Consume(ConsumeContext<TestEvent> context) => Task.CompletedTask;
}