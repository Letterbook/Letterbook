using Letterbook.Core.Tests;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Workers.Tests;

public class WithMockBus<T, TImpl> : WithMocks
	where TImpl : class, T
	where T : class
{
	protected ServiceProvider Provider
	{
		get
		{
			_provider ??= Services.BuildServiceProvider();
			return _provider;
		}
	}

	protected readonly ITestHarness Harness;
	protected readonly IServiceCollection Services;

	private ServiceProvider? _provider;


	public WithMockBus()
	{
		Services = new ServiceCollection()
			.AddMocks(this)
			.AddScoped<T, TImpl>()
			.AddMassTransitTestHarness(bus =>
			{
				bus.AddTestBus();
				ConfigureBus(bus);
			});
		Harness = Provider.GetRequiredService<ITestHarness>();
		Harness.Start().Wait();
	}

	protected virtual void ConfigureBus(IBusRegistrationConfigurator bus)
	{
	}
}