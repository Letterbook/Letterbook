﻿using MassTransit.Logging;
using MassTransit.Monitoring;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Letterbook.Hosting;

public static class DependencyInjection
{
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