using System.Reflection;
using Letterbook.Workers.Contracts;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using OpenTelemetry;

namespace Letterbook.Workers;

public static class DependencyInjection
{
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
		bus.UsingInMemory((context, configurator) => configurator.ConfigureEndpoints(context));

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
			.Where(p => typeof(IConsumer<EventBase>).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
			.ToArray();

		bus.AddConsumers(consumers);
		bus.AddSagaStateMachines(entryAssembly);
		bus.AddSagas(entryAssembly);
		bus.AddActivities(entryAssembly);

		return bus;
	}

	public static OpenTelemetryBuilder AddWorkerTelemetry(this OpenTelemetryBuilder builder)
	{
		return builder.WithMetrics(metrics =>
			{
				metrics.AddMeter(InstrumentationOptions.MeterName);
			})
			.WithTracing(tracing =>
			{
				tracing.AddSource(DiagnosticHeaders.DefaultListenerName);
			});
	}
}