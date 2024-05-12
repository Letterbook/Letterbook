using ActivityPub.Types;
using Letterbook.Core.Adapters;
using Letterbook.Core.Authorization;
using Letterbook.Core.Events;
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

		// Register Mapping Configs
		// services.AddSingleton<MappingConfigProvider>();

		// Register Services
		services.AddScoped<IProfileEventService, ProfileEventService>();
		services.AddScoped<IAccountService, AccountService>();
		services.AddScoped<IProfileService, ProfileService>();
		services.AddScoped<IPostService, PostService>();
		services.AddScoped<IAccountEvents, AccountEventService>();
		services.AddScoped<IActivityMessage, ActivityMessageService>();
		services.AddScoped<IAuthzPostService, PostService>();
		services.AddScoped<IPostEvents, PostEventService>();
		services.AddSingleton<IAuthorizationService, AuthorizationService>();
		services.AddSingleton<IHostSigningKeyProvider, DevelopmentHostSigningKeyProvider>();

		// Register service workers
		services.AddScoped<SeedAdminWorker>();
		services.AddHostedService<WorkerScope<SeedAdminWorker>>();

		// Register MessageWorkers
		// services.AddScoped<DeliveryWorker>();
		// services.AddSingleton<IEventObserver<DeliveryWorker>, EventObserver<DeliveryWorker>>();
		// services.AddHostedService<ObserverHost<IActivityMessage, DeliveryWorker>>(provider =>
		// 	new ObserverHost<IActivityMessage, DeliveryWorker>(provider,
		// 		provider.GetRequiredService<IMessageBusClient>(),
		// 		50));

		// Register Adapters
		// services.AddScoped<IAccountProfileAdapter, AccountProfileAdapter>();
		// services.AddScoped<IActivityAdapter, ActivityAdapter>();
		// services.AddScoped<IPostAdapter, PostAdapter>();
		// services.AddRxMessageBus();
		// services.AddSingleton<IActivityPubDocument, Document>();
		// services.AddDbAdapter(configuration.GetSection(DbOptions.ConfigKey));
		// services.AddDbContext<FeedsContext>();
		// services.TryAddTypesModule();

		// Register HTTP signature authentication services
		// services.AddScoped<IVerificationKeyProvider, ActivityPubClientVerificationKeyProvider>();

		return services;
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