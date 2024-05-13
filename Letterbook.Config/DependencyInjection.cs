using Letterbook.Adapter.Db;
using Letterbook.Core.Models;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Letterbook.Config;

public static class DependencyInjection
{
	// TODO: in-memory bus is only valid for the unified host
	// All other configurations must use an external bus
	// (Meaning this doesn't work for the stand alone API, web, or workers)
	public static IBusRegistrationConfigurator AddWorkerBus(this IBusRegistrationConfigurator bus, IConfigurationManager config)
	{
		var workerOpts = config.GetSection(WorkerOptions.ConfigKey).Get<WorkerOptions>();
		bus.UsingInMemory((context, configurator) => configurator.ConfigureEndpoints(context));

		return bus;
	}

	public static IdentityBuilder AddIdentity(this IServiceCollection services)
	{
		return services.AddIdentity<Account, IdentityRole<Guid>>(options =>
			{

			})
			.AddEntityFrameworkStores<RelationalContext>()
			.AddDefaultTokenProviders();
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

	public static OpenTelemetryBuilder AddDbTelemetry(this OpenTelemetryBuilder builder)
	{
		return builder.WithMetrics(metrics =>
			{
				metrics.AddMeter("Npgsql");
			})
			.WithTracing(tracing =>
			{
				tracing.AddNpgsql();
			});
	}

	public static OpenTelemetryBuilder AddClientTelemetry(this OpenTelemetryBuilder builder)
	{
		return builder.WithMetrics(metrics =>
			{
				metrics.AddHttpClientInstrumentation();
			})
			.WithTracing(tracing =>
			{
				tracing.AddHttpClientInstrumentation();
			});
	}

	public static OpenTelemetryBuilder AddTelemetryExporters(this OpenTelemetryBuilder builder)
	{
		return builder.WithMetrics(metrics =>
			{
				metrics.AddPrometheusExporter();
			})
			.WithTracing(tracing =>
			{
				tracing.AddOtlpExporter();
			});
	}
}