using ActivityPub.Types;
using Letterbook.Core.Adapters;
using Letterbook.Core.Tests;
using Letterbook.Workers.Consumers;
using Letterbook.Workers.Publishers;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Workers.Tests;

public class AccountEventTests : WithMocks
{
	private readonly ServiceProvider _provider;
	private readonly IActivityMessagePublisher _publisher;

	public AccountEventTests()
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

		MockAuthorizeAllowAll();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_publisher);
		Assert.Fail();
	}
}