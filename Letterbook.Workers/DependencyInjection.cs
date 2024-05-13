using System.Reflection;
using Letterbook.Config;
using Letterbook.Core.Contracts;
using MassTransit;

namespace Letterbook.Workers;

public static class DependencyInjection
{
	public static IServiceCollection AddLetterbookWorkers(this IServiceCollection services, IConfigurationManager config)
	{
		var workerOpts = config.GetSection(WorkerOptions.ConfigKey).Get<WorkerOptions>();
		return services.AddMassTransit(x =>
		{
			x.SetKebabCaseEndpointNameFormatter();

			// By default, sagas are in-memory, but should be changed to a durable
			// saga repository.
			x.SetInMemorySagaRepositoryProvider();

			var entryAssembly = Assembly.GetExecutingAssembly();
			var consumers = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => typeof(IConsumer<EventBase>).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
				.ToArray();

			x.AddConsumers(consumers);
			x.AddSagaStateMachines(entryAssembly);
			x.AddSagas(entryAssembly);
			x.AddActivities(entryAssembly);

			x.AddWorkerBus(config);
		});
	}
}