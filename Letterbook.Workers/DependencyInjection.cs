using System.Reflection;
using System.Text.Json.Serialization;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using OpenTelemetry;

namespace Letterbook.Workers;

public static class DependencyInjection
{
	/// <summary>
	/// Registers the message publishing services. All host projects will need to do this.
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection AddPublishers(this IServiceCollection services)
	{
		return services.AddScoped<IActivityMessagePublisher, ActivityMessagePublisher>()
			.AddSingleton<IApCrawlScheduler, ApCrawlScheduler>()
			.AddScoped<IAccountEventPublisher, AccountEventPublisher>()
			.AddScoped<IPostEventPublisher, PostEventPublisher>()
			.AddScoped<IProfileEventPublisher, ProfileEventPublisher>();
	}

	/// <summary>
	/// Configures the shared message bus. All host projects will likely need to do this,
	/// because all host projects need to put messages on the bus
	/// </summary>
	/// <param name="bus"></param>
	/// <param name="config"></param>
	/// <returns></returns>
	public static IBusRegistrationConfigurator AddWorkerBus(this IBusRegistrationConfigurator bus, IConfigurationManager config)
	{
		var workerOpts = config.GetSection(WorkerOptions.ConfigKey).Get<WorkerOptions>();
		bus.UsingInMemory((context, configurator) =>
		{
			configurator.ConfigureEndpoints(context);
			configurator.ConfigureJsonSerializerOptions(options => options.AddDtoSerializer());
		});

		return bus;
	}

	/// <summary>
	/// Configures the workers and channels. Only projects hosting workers need to do this.
	/// </summary>
	/// <param name="bus"></param>
	/// <param name="config"></param>
	/// <returns></returns>
	public static IBusRegistrationConfigurator AddWorkers(this IBusRegistrationConfigurator bus, IConfigurationManager config)
	{
		var workerOpts = config.GetSection(WorkerOptions.ConfigKey).Get<WorkerOptions>();
		bus.SetKebabCaseEndpointNameFormatter();

		// By default, sagas are in-memory, but should be changed to a durable
		// saga repository.
		bus.SetInMemorySagaRepositoryProvider();

		var entryAssembly = Assembly.GetExecutingAssembly();
		var consumers = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes())
			.Where(p => typeof(IConsumer).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract &&
			            (p.Namespace?.StartsWith("Letterbook") ?? false))
			.ToArray();

		bus.AddConsumers(consumers);
		bus.AddSagaStateMachines(entryAssembly);
		bus.AddSagas(entryAssembly);
		bus.AddActivities(entryAssembly);

		return bus;
	}

	public static OpenTelemetryBuilder AddWorkerTelemetry(this OpenTelemetryBuilder builder)
	{
		return builder.WithMetrics(metrics => { metrics.AddMeter(InstrumentationOptions.MeterName); })
			.WithTracing(tracing => { tracing.AddSource(DiagnosticHeaders.DefaultListenerName); });
	}
}