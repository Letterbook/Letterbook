using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ActivityPub.Types;
using DarkLink.Web.WebFinger.Server;
using DarkLink.Web.WebFinger.Shared;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.ActivityPub.Signatures;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Api.Authentication.HttpSignature.DependencyInjection;
using Letterbook.Api.Authentication.HttpSignature.Handler;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Authorization;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Constants = DarkLink.Web.WebFinger.Shared.Constants;

namespace Letterbook.Api;

public static class DependencyInjectionExtensions
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		Converters = { new JsonResourceDescriptorConverter(), },
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	public static IServiceCollection AddWebfinger(this IServiceCollection services)
	{
		return services.AddScoped<IResourceDescriptorProvider, WebfingerProvider>();
	}

	public static void UseWebFingerScoped(this IApplicationBuilder app)
	{
		app.Map(
			Constants.HTTP_PATH,
			app => app.Run(async ctx =>
			{
				if (!ctx.Request.Query.TryGetValue(Constants.QUERY_RESOURCE, out var resourceRaw)
				    || !Uri.TryCreate(resourceRaw, UriKind.RelativeOrAbsolute, out var resource))
				{
					ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
					return;
				}

				using var scope = app.ApplicationServices.CreateScope();
				var resourceDescriptorProvider =
					scope.ServiceProvider.GetRequiredService<IResourceDescriptorProvider>();

				ctx.Request.Query.TryGetValue(Constants.QUERY_RELATION, out var relations);

				var descriptor =
					await resourceDescriptorProvider.GetResourceDescriptorAsync(resource, relations, ctx.Request, ctx.RequestAborted);
				if (descriptor is null)
				{
					ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
					return;
				}

				await ctx.Response.WriteAsJsonAsync(
					descriptor,
					JsonSerializerOptions,
					Constants.MEDIA_TYPE,
					ctx.RequestAborted);
			}));
	}


	public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
	{
		// Register options
		services.Configure<CoreOptions>(configuration.GetSection(CoreOptions.ConfigKey));
		services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.ConfigKey));
		services.Configure<DbOptions>(configuration.GetSection(DbOptions.ConfigKey));

		// Register Mapping Configs
		services.AddSingleton<MappingConfigProvider>();

		// Register Services
		services.AddScoped<IProfileEventService, ProfileEventService>();
		services.AddScoped<IAccountService, AccountService>();
		services.AddScoped<IProfileService, ProfileService>();
		services.AddScoped<IPostService, PostService>();
		services.AddScoped<IAccountProfileAdapter, AccountProfileAdapter>();
		services.AddScoped<IAuthzPostService, PostService>();
		services.AddSingleton<IAuthorizationService, AuthorizationService>();

		// Register startup workers
		services.AddScopedService<SeedAdminWorker>();

		// Register Adapters
		services.AddScoped<IActivityAdapter, ActivityAdapter>();
		services.AddScoped<IPostAdapter, PostAdapter>();
		services.AddSingleton<IActivityPubDocument, Document>();
		services.AddDbAdapter(configuration);
		services.AddFeedsAdapter(configuration);
		services.TryAddTypesModule();

		// Register HTTP signature authentication services
		services.AddSingleton<IHostSigningKeyProvider, DevelopmentHostSigningKeyProvider>();
		services.AddScoped<IVerificationKeyProvider, ActivityPubClientVerificationKeyProvider>();

		return services;
	}

	public static OpenTelemetryBuilder AddTelemetry(this IServiceCollection services)
	{
		return services.AddOpenTelemetry()
			.ConfigureResource(resource => { resource.AddService("Letterbook"); })
			.WithMetrics(metrics =>
			{
				metrics.AddAspNetCoreInstrumentation();
				metrics.AddHttpClientInstrumentation();
				metrics.AddMeter("Npgsql");
				metrics.AddPrometheusExporter();
			})
			.WithTracing(tracing =>
			{
				tracing.AddAspNetCoreInstrumentation();
				tracing.AddHttpClientInstrumentation();
				tracing.AddNpgsql();
				tracing.AddOtlpExporter();
			});
	}

	public static IServiceCollection AddApiProperties(this IServiceCollection services, ConfigurationManager configuration)
	{
		var coreOptions = configuration.GetSection(CoreOptions.ConfigKey).Get<CoreOptions>()
		                  ?? throw new ConfigException(nameof(CoreOptions));
		// Register controllers
		services
			.ConfigureSwagger()
			.AddControllers(options =>
			{
				options.ModelBinderProviders.Insert(0, new Uuid7BinderProvider());
				options.Conventions.Add(new RouteTokenTransformerConvention(new SnakeCaseRouteTransformer()));
				options.OutputFormatters.Insert(0, new JsonLdOutputFormatter());
				options.InputFormatters.Insert(0, new JsonLdInputFormatter());
			})
			.AddJsonOptions(options => options.JsonSerializerOptions.AddDtoSerializer())
			.Services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
					var problemDetails = new ValidationProblemDetails(context.ModelState)
					{
						Instance = context.HttpContext.Request.Path,
						Status = (int)HttpStatusCode.BadRequest,
					};
					logger.LogInformation("Validation failed {@Problem}", problemDetails);

					return new BadRequestObjectResult(problemDetails);
				};
			})
			.AddWebfinger()
			// Register Authentication
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			{
				options.Authority = coreOptions.BaseUri().ToString();
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateAudience = false
				};
				// TODO(Security): Figure out how to do this only in Development
				options.RequireHttpsMetadata = false;
			})
			.AddHttpSignature();

		services.AddAuthorization(opts =>
		{
			opts.AddPolicy("ActivityPub", static policy =>
			{
				policy.RequireAuthenticatedUser();
				policy.AddAuthenticationSchemes(HttpSignatureAuthenticationDefaults.Scheme);
			});
		});

		return services;
	}
}