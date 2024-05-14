using Letterbook.Adapter.ActivityPub;
using Letterbook.Api;
using Letterbook.Config;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Letterbook.Web;

public class Program
{
	public static void Main(string[] args)
	{
		// Pre initialize Serilog
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.WriteTo.Console()
			.CreateBootstrapLogger();

		var builder = WebApplication.CreateBuilder(args);
		var coreOptions = builder.Configuration.GetSection(CoreOptions.ConfigKey).Get<CoreOptions>()
		                  ?? throw new ConfigException(nameof(CoreOptions));

		if (!builder.Environment.IsProduction())
			builder.Configuration.AddUserSecrets<Api.Program>();
		// Register Serilog - Serialized Logging (configured in appsettings.json)
		builder.Host.UseSerilog((context, services, configuration) => configuration
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
		);

		builder.Services.AddApiProperties(builder.Configuration);
		builder.Services.AddTelemetry();
		builder.Services.AddHealthChecks();
		builder.Services.AddActivityPubClient(builder.Configuration);
		builder.Services.AddServices(builder.Configuration);
		builder.Services.AddIdentity()
			// Aspnet Core Identity includes a default UI, which provides basic account management.
			// This includes register, login, logout, and maintenance views.
			// However, it only seems to work well for fairly simple (single project) apps.  It also looks extremely Microsoft.
			// We likely don't want to use it long term, but it's nice for the moment.
			//
			// We can work around some of the issues by overriding pages under Areas/IdentityPages/Account
			.AddDefaultUI();
		builder.Services.AddRazorPages();
		builder.Services.AddMassTransit(bus => bus.AddWorkerBus(builder.Configuration));

		builder.WebHost.UseUrls(coreOptions.BaseUri().ToString());

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			// Not sure if this works, with mixed Razor/WebApi
			app.UseExceptionHandler("/Error");
		}

		if (!app.Environment.IsProduction())
		{
			app.Use((context, next) =>
			{
				context.Request.EnableBuffering();
				return next();
			});
		}

		app.UseStaticFiles();

		app.UseHealthChecks("/healthz");
		app.MapPrometheusScrapingEndpoint();
		app.UseWebFingerScoped();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseSerilogRequestLogging();

		app.MapRazorPages();
		app.UsePathBase(new PathString("/api/v1"));

		app.Run("http://localhost:5127");
	}
}