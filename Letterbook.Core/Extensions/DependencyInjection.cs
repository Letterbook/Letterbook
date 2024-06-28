using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Core.Adapters;
using Letterbook.Core.Authorization;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Models.Mappers.Converters;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
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
		services.AddScoped<IProfileEventService, ProfileEventService>()
			.AddScoped<IAccountService, AccountService>()
			.AddScoped<IProfileService, ProfileService>()
			.AddScoped<IPostService, PostService>()
			.AddScoped<IAuthzPostService, PostService>()
			.AddScoped<ITimelineService, TimelineService>()
			.AddScoped<MappingConfigProvider>()
			.AddSingleton<IAuthorizationService, AuthorizationService>()
			.AddSingleton<IHostSigningKeyProvider, DevelopmentHostSigningKeyProvider>();

		// Register service workers
		services.AddScopedService<SeedAdminWorker>()
			.AddHostedService<HostLifetimeWorker>();

		return services;
	}

	public static IServiceCollection AddScopedService<TScopedWorker>(this IServiceCollection services)
		where TScopedWorker : class, IScopedWorker
	{
		return services.AddHostedService<WorkerScope<TScopedWorker>>()
			.AddScoped<TScopedWorker>();
	}

	public static IOpenTelemetryBuilder AddClientTelemetry(this IOpenTelemetryBuilder builder)
	{
		return builder
			.WithMetrics(metrics => { metrics.AddHttpClientInstrumentation(); })
			.WithTracing(tracing => { tracing.AddHttpClientInstrumentation(); });
	}

	public static IOpenTelemetryBuilder AddTelemetryExporters(this IOpenTelemetryBuilder builder)
	{
		return builder
			.WithMetrics(metrics => { metrics.AddPrometheusExporter(); })
			.WithTracing(tracing => { tracing.AddOtlpExporter(); });
	}

	public static JsonSerializerOptions AddDtoSerializer(this JsonSerializerOptions options)
	{
		options.Converters.Add(new Uuid7JsonConverter());
		options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

		return options;
	}

	public static void ConfigureIdentity(this IdentityOptions identity)
	{
		// Removes "@" from the default list, which would likely cause problems for other services using webfinger
		identity.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._+";
		identity.User.RequireUniqueEmail = true;
		identity.ClaimsIdentity.EmailClaimType = JwtRegisteredClaimNames.Email;
		identity.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
		identity.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.PreferredUsername;
	}
}