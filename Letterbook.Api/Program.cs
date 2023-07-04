using Letterbook.Adapter.Db;
using Letterbook.Adapter.FediClient;
using Letterbook.Core;
using Letterbook.Core.Ports;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Letterbook.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new SnakeCaseRouteTransformer()));
        });
        
        // Register DI config
        builder.Services.Configure<ConfigOptions>(builder.Configuration.GetSection(ConfigOptions.Key));
        builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.ConfigKey));
        
        // Register DI containers
        builder.Services.AddScoped<IActivityService, ActivityService>();
        builder.Services.AddScoped<IFediAdapter, FediClient>();
        builder.Services.AddScoped<IActivityAdapter, ActivityAdapter>();
        builder.Services.AddScoped<IShareAdapter, ActivityAdapter>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.UsePathBase(new PathString("/api/v1"));


        app.MapControllers();

        app.Run();
    }
}
