using System.Net;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.RxMessageBus;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Api.Dto;
using Letterbook.Api.Mappers;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Authorization;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Events;

namespace Letterbook.Api;

public class Program
{
    public static void Main(string[] args)
    {
        // Pre initialize Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        var builder = WebApplication.CreateBuilder(args);
        var coreSection = builder.Configuration.GetSection(CoreOptions.ConfigKey);
        var coreOptions = coreSection.Get<CoreOptions>()
                          ?? throw new ArgumentException("Invalid configuration", nameof(CoreOptions));

        if (!builder.Environment.IsProduction())
            builder.Configuration.AddUserSecrets<Program>();
        // Register Serilog - Serialized Logging (configured in appsettings.json)
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));

        // Register controllers
        builder.Services
            .AddControllers(options =>
            {
	            options.ModelBinderProviders.Insert(0, new Uuid7BinderProvider());
                options.Conventions.Add(new RouteTokenTransformerConvention(new SnakeCaseRouteTransformer()));
                options.OutputFormatters.Insert(0, new JsonLdOutputFormatter());
                options.InputFormatters.Insert(0, new JsonLdInputFormatter());
            })
            .AddJsonOptions(options =>
            {
	            options.JsonSerializerOptions.Converters.Add(new Json.Uuid7JsonConverter());
            })
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
            });
        builder.Services.AddWebfinger();

        // Register Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = coreOptions.BaseUri().ToString();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
                // TODO(Security): Figure out how to do this only in Development
                options.RequireHttpsMetadata = false;
            });

        // Register Open Telemetry
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddPrometheusExporter();
            });

        builder.Services.AddActivityPubClient(coreOptions.DomainName);

        // Register options
        builder.Services.Configure<CoreOptions>(coreSection);
        builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.ConfigKey));
        builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.ConfigKey));

        // Register Mapping Configs
        builder.Services.AddSingleton<MappingConfigProvider>();
        builder.Services.AddSingleton<BaseMappings>();

        // Register Services
        builder.Services.AddScoped<IActivityEventService, ActivityEventService>();
        builder.Services.AddScoped<IProfileEventService, ProfileEventService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<IAccountEventService, AccountEventService>();
        builder.Services.AddScoped<IAccountProfileAdapter, AccountProfileAdapter>();
        builder.Services.AddScoped<IActivityMessageService, ActivityMessageService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IPostEventService, PostEventService>();
        builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();

        // Register Workers
        builder.Services.AddScoped<SeedAdminWorker>();
        builder.Services.AddScoped<DeliveryWorker>();
        builder.Services.AddSingleton<DeliveryObserver>();
        builder.Services.AddHostedService<WorkerScope<SeedAdminWorker>>();
        builder.Services.AddHostedService<MessageWorkerHost<DeliveryObserver, ASType>>((DeliveryObserverFactory));

        // Register Adapters
        builder.Services.AddScoped<IActivityAdapter, ActivityAdapter>();
        builder.Services.AddScoped<IPostAdapter, PostAdapter>();
        builder.Services.AddRxMessageBus();
        builder.Services.AddSingleton<IActivityPubDocument, Document>();
        builder.Services.AddDbContext<RelationalContext>();
        builder.Services.AddDbContext<FeedsContext>();
        builder.Services.AddIdentity<Account, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<RelationalContext>();
        builder.Services.TryAddTypesModule();

        builder.Services.ConfigureSwagger();

        builder.WebHost.UseUrls(coreOptions.BaseUri().ToString());
        var app = builder.Build();
        // Configure the HTTP request pipeline.

        // Add development niceties
        if (!app.Environment.IsProduction())
        {
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseSwaggerConfig();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseWebFingerScoped();

        app.MapPrometheusScrapingEndpoint();

        app.UseSerilogRequestLogging();

        app.UsePathBase(new PathString("/api/v1"));
        app.MapControllers();

        app.Run("http://localhost:5127");
    }

    private static MessageWorkerHost<DeliveryObserver, ASType> DeliveryObserverFactory(IServiceProvider provider)
    {
        // Set a 50ms delay on the delivery observer
        // This is just a guess at a sufficient amount of time for peers to become ready to accept reply messages
        // We don't want to sit on them for too long, because they'll just sitting in RAM
        return new MessageWorkerHost<DeliveryObserver, ASType>(
            provider.GetRequiredService<ILogger<MessageWorkerHost<DeliveryObserver, ASType>>>(), provider,
            provider.GetRequiredService<IMessageBusClient>(), 50);
    }
}