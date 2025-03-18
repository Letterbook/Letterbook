using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Api.Authentication.HttpSignature;
using Letterbook.Api.Authentication.HttpSignature.DependencyInjection;
using Letterbook.Api.Swagger;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Workers;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Letterbook.Api;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Pre initialize Serilog boostrap logger
		// if(builder.Environment.IsProduction())
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.ReadFrom.Configuration(builder.Configuration)
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.WriteTo.Console()
			.CreateBootstrapLogger();

		builder.ConfigureHostBuilder();

		var coreOptions = builder.Configuration.GetSection(CoreOptions.ConfigKey).Get<CoreOptions>()
		                  ?? throw ConfigException.Missing(nameof(CoreOptions));
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

		app.UseHealthChecks("/healthz");
		app.MapPrometheusScrapingEndpoint();
		app.UseWebFingerScoped();

		app.UseHttpSignatureVerification();
		app.UseAuthentication();
		app.UseAuthorization();

		app.UseSerilogRequestLogging();

		app.UsePathBase(new PathString("/api/v1"));
		app.MapControllers();

		app.Run();
	}
}