using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.RxMessageBus;
using Letterbook.Adapter.TimescaleFeeds;
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

namespace Letterbook.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var coreSection = builder.Configuration.GetSection(CoreOptions.ConfigKey);
        var coreOptions = coreSection.Get<CoreOptions>() 
                   ?? throw new ArgumentException("Invalid configuration", nameof(CoreOptions));

        // Register controllers
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SnakeCaseRouteTransformer()));
        }).AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        // Register Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = coreOptions.BaseUri().ToString();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });
        
        // Register options
        builder.Services.Configure<CoreOptions>(coreSection);
        builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.ConfigKey));
        builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.ConfigKey));
        
        // Register Services
        builder.Services.AddScoped<IActivityService, ActivityService>();
        builder.Services.AddScoped<IActivityEventService, ActivityEventService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IAccountEventService, AccountEventService>();
        builder.Services.AddScoped<IAccountProfileAdapter, AccountProfileAdapter>();
        
        // Register Workers
        builder.Services.AddScoped<IScopedWorker, SeedAdminWorker>();
        builder.Services.AddHostedService<WorkerScope<SeedAdminWorker>>();
        // TODO: clean up and make things buildable again, then see if you can log in
        
        // Register Adapters
        builder.Services.AddScoped<IActivityAdapter, ActivityAdapter>();
        builder.Services.AddSingleton<IMessageBusAdapter, RxMessageBus>();
        builder.Services.AddDbContext<RelationalContext>();
        builder.Services.AddDbContext<FeedsContext>();
        builder.Services.AddIdentity<AccountIdentity, IdentityRole<Guid>>()
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

        });

        builder.WebHost.UseUrls(coreOptions.BaseUri().ToString());
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UsePathBase(new PathString("/api/v1"));

        app.MapControllers();

        app.Run();
    }
}
