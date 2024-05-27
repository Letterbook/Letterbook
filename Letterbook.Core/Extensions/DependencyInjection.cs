using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Core.Adapters;
using Letterbook.Core.Authorization;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.Core.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Letterbook.Core.Extensions;

public static class DependencyInjection
{
	public static IServiceCollection AddLetterbookCore(this IServiceCollection services, IConfigurationManager config)
	{
		// Register options
		services.Configure<CoreOptions>(config.GetSection(CoreOptions.ConfigKey));

		// Register Services
		services.AddScoped<IProfileEventService, ProfileEventService>();
		services.AddScoped<IAccountService, AccountService>();
		services.AddScoped<IProfileService, ProfileService>();
		services.AddScoped<IPostService, PostService>();
		services.AddScoped<IAuthzPostService, PostService>();
		services.AddSingleton<IAuthorizationService, AuthorizationService>();
		services.AddSingleton<IHostSigningKeyProvider, DevelopmentHostSigningKeyProvider>();

		// Register service workers
		services.AddScopedService<SeedAdminWorker>();

		return services;
	}

	public static IServiceCollection AddScopedService<TScopedWorker>(this IServiceCollection services)
		where TScopedWorker : class, IScopedWorker
	{

		return services.AddHostedService<WorkerScope<TScopedWorker>>()
			.AddScoped<TScopedWorker>();
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

	public static JsonSerializerOptions AddDtoSerializer(this JsonSerializerOptions options)
	{
		options.Converters.Add(new Uuid7JsonConverter());
		options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

		return options;
	}
}