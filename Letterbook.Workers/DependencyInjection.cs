using System.Reflection;
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
		return services.AddMassTransit(x =>
		{
			x.SetKebabCaseEndpointNameFormatter();

			// By default, sagas are in-memory, but should be changed to a durable
			// saga repository.
			x.SetInMemorySagaRepositoryProvider();

			var entryAssembly = Assembly.GetEntryAssembly();

			x.AddConsumers(entryAssembly);
			x.AddSagaStateMachines(entryAssembly);
			x.AddSagas(entryAssembly);
			x.AddActivities(entryAssembly);

			x.UsingInMemory((context, cfg) =>
			{
				cfg.ConfigureEndpoints(context);
			});
		});
	}

	// public static OpenTelemetryBuilder AddWorkerTelemetry(this OpenTelemetryBuilder builder)
	// {
	// 	return builder.WithMetrics(metrics =>
	// 		{
	// 			metrics.AddMeter(InstrumentationOptions.MeterName);
	// 		})
	// 		.WithTracing(tracing =>
	// 		{
	// 			tracing.AddSource(DiagnosticHeaders.DefaultListenerName);
	// 		});
	// }
	//
	// public static OpenTelemetryBuilder AddDbTelemetry(this OpenTelemetryBuilder builder)
	// {
	// 	return builder.WithMetrics(metrics =>
	// 		{
	// 			metrics.AddMeter("Npgsql");
	// 		})
	// 		.WithTracing(tracing =>
	// 		{
	// 			tracing.AddNpgsql();
	// 		});
	// }
	//
	// public static OpenTelemetryBuilder AddClientTelemetry(this OpenTelemetryBuilder builder)
	// {
	// 	return builder.WithMetrics(metrics =>
	// 		{
	// 			metrics.AddHttpClientInstrumentation();
	// 		})
	// 		.WithTracing(tracing =>
	// 		{
	// 			tracing.AddHttpClientInstrumentation();
	// 		});
	// }
	//
	// public static OpenTelemetryBuilder AddTelemetryExporters(this OpenTelemetryBuilder builder)
	// {
	// 	return builder.WithMetrics(metrics =>
	// 		{
	// 			metrics.AddPrometheusExporter();
	// 		})
	// 		.WithTracing(tracing =>
	// 		{
	// 			tracing.AddOtlpExporter();
	// 		});
	// }
}