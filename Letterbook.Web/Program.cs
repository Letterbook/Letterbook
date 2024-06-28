using System.Text.Encodings.Web;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Workers;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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
		builder.Services.AddRazorPages();
		builder.Services.AddAuthorization(options =>
		{
			options.AddWebAuthzPolicy();
		});

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

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseSerilogRequestLogging();

		app.MapRazorPages();

		app.Run("http://localhost:5127");
	}
}

public class WebSchemeHandler : AuthenticationHandler<WebSchemeOptions>
{

	public WebSchemeHandler(IOptionsMonitor<WebSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
	{
	}
	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		throw new NotImplementedException();
	}
}

public class WebSchemeOptions : AuthenticationSchemeOptions
{
}