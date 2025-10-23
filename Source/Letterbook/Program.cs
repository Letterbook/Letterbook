using System.Reflection;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Api;
using Letterbook.Api.Authentication.HttpSignature.DependencyInjection;
using Letterbook.Api.Swagger;
using Letterbook.AspNet;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Web;
using Letterbook.Workers;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Letterbook;

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

		if (!builder.Environment.IsProduction())
			builder.Configuration.AddUserSecrets<Api.Program>();
		// Register Serilog - Serialized Logging (configured in appsettings.json)
		builder.Host.UseSerilog((context, services, configuration) => configuration
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services),
			preserveStaticLogger: true
		);

		builder.Services.AddOpenTelemetry()
			.AddTelemetry()
			.AddAspnetTelemetry()
			.AddWorkerTelemetry()
			.AddDbTelemetry();
		builder.Services.AddHealthChecks();
		builder.Services.AddLetterbookCore(builder.Configuration)
			.AddActivityPubClient(builder.Configuration)
			.AddApiProperties(builder.Configuration)
			.AddPublishers()
			.AddDbAdapter(builder.Configuration)
			.AddFeedsAdapter(builder.Configuration)
			.AddWebCookies()
			.AddAspnetServices();
		builder.Services.AddIdentity<Account, IdentityRole<Guid>>(identity => identity.ConfigureIdentity())
			.AddEntityFrameworkStores<RelationalContext>()
			.AddDefaultTokenProviders()
			.AddDefaultUI();
		builder.Services.ConfigureAccountManagement(builder.Configuration);
		builder.Services.AddRazorPages()
			.AddApplicationPart(Assembly.GetAssembly(typeof(Web.Program))!)
			.AddRazorPagesOptions(options => options.AddWebRoutes());
		builder.Services.Configure<RouteOptions>(opts => opts.ConfigureWebRoutes());
		builder.Services.AddAuthorization(options =>
		{
			options.AddWebAuthzPolicy();
			options.AddpiAuthzPolicy();
		});
		builder.Services.AddMassTransit(bus => bus.AddWorkerBus(builder.Configuration)
			.AddWorkers(builder.Configuration));

		// builder.WebHost.UseUrls(coreOptions.BaseUri().ToString());

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
			app.UseSwaggerConfig();
		}

		// app.UseHttpsRedirection();
		app.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new ManifestEmbeddedFileProvider(Assembly.GetAssembly(typeof(Web.Program))!, "wwwroot"),
		});
		app.UseStatusCodePagesWithReExecute("/error/{0}");

		app.UseHealthChecks("/healthz");
		app.MapPrometheusScrapingEndpoint();

		app.UseHttpSignatureVerification();
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseWhen(ProfileIdentityPaths, applicationBuilder =>
		{
			applicationBuilder.UseMiddleware<ProfileIdentityMiddleware>();
		});

		app.UseSerilogRequestLogging();

		app.UseWebFingerScoped();
		app.MapRazorPages();
		app.UsePathBase(new PathString("/api/v1"));
		app.MapControllers();

		var options = app.Services.GetRequiredService<IOptions<CoreOptions>>().Value;
		MigrateAtRuntime(options);

		app.Run();

		return;

		void MigrateAtRuntime(CoreOptions coreOptions)
		{
			if (!coreOptions.Database.MigrateAtRuntime) return;
			using var scope = app.Services.CreateScope();
			Log.Logger.Information("Migrating primary database...");
			var data = scope.ServiceProvider.GetRequiredService<RelationalContext>();
			data.Database.Migrate();
			Log.Logger.Information("Migrating primary database... Done");


			Log.Logger.Information("Migrating feeds database...");
			var feeds = scope.ServiceProvider.GetRequiredService<FeedsContext>();
			feeds.Database.Migrate();
			Log.Logger.Information("Migrating feeds database... Done");
		}

		static bool ProfileIdentityPaths(HttpContext context)
		{
			return !context.Request.Path.StartsWithSegments("/Identity")
			       // TODO: prefix with /ap/v1
			       // Need to use url generators integrated with routing, instead of just a bunch of magic strings
			       && !context.Request.Path.StartsWithSegments("/actor")
			       && !context.Request.Path.StartsWithSegments("/object")
			       && !context.Request.Path.StartsWithSegments("/.well-known");
		}
	}
}