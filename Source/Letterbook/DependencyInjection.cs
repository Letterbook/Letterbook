using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Letterbook;

public static class DependencyInjection
{
	public static OpenTelemetryBuilder AddAspnetTelemetry(this OpenTelemetryBuilder telemetry)
	{
		return telemetry.ConfigureResource(resource => { resource.AddService("Letterbook"); })
			.WithMetrics(metrics =>
			{
				metrics.AddAspNetCoreInstrumentation();
			})
			.WithTracing(tracing =>
			{
				tracing.AddAspNetCoreInstrumentation();
			});
	}
}