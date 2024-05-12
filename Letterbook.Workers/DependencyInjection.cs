using System.Reflection;
using Letterbook.Core.Contracts;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Letterbook.Workers;

public static class DependencyInjection
{
	public static IServiceCollection AddLetterbookWorkers(this IServiceCollection services, IConfigurationSection workerConfig)
	{
		var workerOpts = workerConfig.Get<WorkerOptions>();
		return services.AddMassTransit(x =>
		{
			x.SetKebabCaseEndpointNameFormatter();

			// By default, sagas are in-memory, but should be changed to a durable
			// saga repository.
			x.SetInMemorySagaRepositoryProvider();

			// var entryAssembly = Assembly.GetExecutingAssembly();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => typeof(IConsumer<EventBase>).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
				.Select(x => x.Assembly)
				.ToArray();

			x.AddConsumers(assemblies);
			x.AddSagaStateMachines(assemblies);
			x.AddSagas(assemblies);
			x.AddActivities(assemblies);

			x.UsingInMemory((context, cfg) =>
			{
				cfg.ConfigureEndpoints(context);
			});
		});
	}
}