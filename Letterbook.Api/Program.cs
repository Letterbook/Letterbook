using DarkLink.Web.WebFinger.Server;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.RxMessageBus;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

        // Register Serilog - Serialized Logging (configured in appsettings.json)
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));
        
        // Register controllers
        builder.Services
            .AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SnakeCaseRouteTransformer()));
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

        // Configure Http Signatures
        // Note: Starting to move DI config out into adapter as much as possible.
        // I want to reduce how tightly coupled the process hosting component is from the project that handles API
        // routes and controllers. Partly because it feels cleaner.
        // Mostly because high-availability and/or horizontally scaled deployments
        // will have more than one process, likely on more than one host.
        //
        // Workers (ex: SeedAdminWorker and future queue or maintenance workers) should be able to scale independently
        // of the API, and shouldn't need a whole AspNetCore service just to host them.
        // This helps with that.
        builder.Services.AddActivityPubClient(coreOptions.DomainName);
        
        // Register options
        builder.Services.Configure<CoreOptions>(coreSection);
        builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.ConfigKey));
        builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.ConfigKey));
        
        // Register Services
        builder.Services.AddScoped<IActivityService, ActivityService>();
        builder.Services.AddScoped<IActivityEventService, ActivityEventService>();
        builder.Services.AddScoped<IProfileEventService, ProfileEventService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<IAccountEventService, AccountEventService>();
        builder.Services.AddScoped<IAccountProfileAdapter, AccountProfileAdapter>();
        
        // Register Workers
        builder.Services.AddScoped<SeedAdminWorker>();
        builder.Services.AddHostedService<WorkerScope<SeedAdminWorker>>();
        
        // Register Adapters
        builder.Services.AddScoped<IActivityAdapter, ActivityAdapter>();
        builder.Services.AddSingleton<IMessageBusAdapter, RxMessageBus>();
        builder.Services.AddDbContext<RelationalContext>();
        builder.Services.AddDbContext<FeedsContext>();
        builder.Services.AddIdentity<Account, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<RelationalContext>();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Authorization header, including the Bearer scheme, like so: `Bearer <JWT>`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
            options.OperationFilter<RequiredHeaders>();
        });

        builder.WebHost.UseUrls(coreOptions.BaseUri().ToString());
        var app = builder.Build();
        // Configure the HTTP request pipeline.

        // Configure development niceties
        if (!app.Environment.IsProduction())
        {
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseSwagger();
            app.UseSwaggerUI();
            // app.Services.GetService<JwtBearerHandler>()
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
}
