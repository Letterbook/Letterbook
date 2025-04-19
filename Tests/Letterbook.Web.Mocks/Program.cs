using System.Reflection;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.AspNet;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Workers;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Letterbook.Web.Mocks;

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
			builder.Configuration.AddUserSecrets<Program>();
		// Register Serilog - Serialized Logging (configured in appsettings.json)
		builder.Host.UseSerilog((context, services, configuration) => configuration
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
		);

		builder.Services.AddOpenTelemetry()
			.AddDbTelemetry()
			.AddClientTelemetry()
			.AddTelemetryExporters();
		builder.Services
			.AddSerilog(s => s.Enrich.FromLogContext()
				.Enrich.WithSpan()
				.ReadFrom.Configuration(builder.Configuration))
			.AddMassTransit(bus => bus.AddWorkers(builder.Configuration)
				.AddWorkerBus(builder.Configuration))
			.AddPublishers()
			.AddLetterbookCore(builder.Configuration)
			.AddDbAdapter(builder.Configuration)
			.AddFeedsAdapter(builder.Configuration)
			.AddActivityPubClient(builder.Configuration)
			.AddWebCookies();
		builder.Services.AddIdentity<Models.Account, IdentityRole<Guid>>(identity => identity.ConfigureIdentity())
			.AddEntityFrameworkStores<RelationalContext>()
			.AddDefaultTokenProviders()
			// Aspnet Core Identity includes a default UI, which provides basic account management.
			// This includes register, login, logout, and maintenance views.
			// However, it only seems to work well for fairly simple (single project) apps.  It also looks extremely Microsoft.
			// We likely don't want to use it long term, but it's nice for the moment.
			//
			// We can work around some of the issues by overriding pages under Areas/IdentityPages/Account
			.AddDefaultUI();
		builder.Services.AddRazorPages()
			.AddApplicationPart(Assembly.GetAssembly(typeof(Web.Program))!)
			.AddRazorPagesOptions(options => options.AddWebRoutes());
		builder.Services.Configure<RouteOptions>(opts => opts.ConfigureWebRoutes());
		builder.Services.AddAuthorization(options =>
		{
			options.AddWebAuthzPolicy();
		});

		// Add mocked services
		builder.Services
			.AddScoped<ProfileService>()
			.AddScoped<IProfileService, MockProfileService>()
			.AddScoped<TimelineService>()
			.AddScoped<ITimelineService, MockTimelineService>();

		var app = builder.Build();

		// app.UseStatusCodePagesWithReExecute("/error/{0}");
		app.UseDeveloperExceptionPage();
		app.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new ManifestEmbeddedFileProvider(Assembly.GetAssembly(typeof(Web.Program))!, "wwwroot"),
		});

		app.UseHealthChecks("/healthz");
		app.MapPrometheusScrapingEndpoint();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseSerilogRequestLogging();

		app.UseWhen(context => !context.Request.Path.StartsWithSegments("/Identity"), applicationBuilder =>
		{
			applicationBuilder.UseMiddleware<ProfileIdentityMiddleware>();
		});

		app.MapRazorPages();

		var options = app.Services.GetRequiredService<IOptions<CoreOptions>>().Value;
		MigrateAtRuntime(options);
		app.Run();
		return;

		void MigrateAtRuntime(CoreOptions options)
		{
			if (!options.Database.MigrateAtRuntime) return;
			using var scope = app.Services.CreateScope();
			var data = scope.ServiceProvider.GetRequiredService<RelationalContext>();
			data.Database.Migrate();

			var feeds = scope.ServiceProvider.GetRequiredService<FeedsContext>();
			feeds.Database.Migrate();
		}
	}
}