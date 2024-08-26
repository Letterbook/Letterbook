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

namespace Letterbook.Workers.Tests;

public class OutboundPostConsumerTests : WithMocks, IAsyncDisposable
{
	public async ValueTask DisposeAsync()
	{
		GC.SuppressFinalize(this);
		await _provider.DisposeAsync();
	}

	private readonly ServiceProvider _provider;
	private readonly IPostEventPublisher _publisher;
	private readonly ITestHarness _harness;
	private readonly Post _post;
	private readonly Profile _profile;

	public OutboundPostConsumerTests()
	{
		_provider = new ServiceCollection()
			.AddMocks(this)
			.AddScoped<IPostEventPublisher, PostEventPublisher>()
			.AddMassTransitTestHarness(bus =>
			{
				bus.AddConsumer<OutboundPostConsumer>();
				bus.AddTestBus();
			})
			.BuildServiceProvider();
		_publisher = _provider.GetRequiredService<IPostEventPublisher>();
		_harness = _provider.GetRequiredService<ITestHarness>();
		_profile = new FakeProfile("letterbook.example").Generate();
		_post = new FakePost(_profile).Generate();

		_harness.Start().Wait();

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
	}

	[Fact(DisplayName = "Should consume post events")]
	public async Task CanConsume()
	{
		await _publisher.Published(_post);
		Assert.NotNull(_harness.Consumed.Select<PostEvent>().AsEnumerable().SingleOrDefault(message => message.Exception is NotImplementedException));
	}
}