using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.AspNet;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Web.Tests.E2E.Support;
using Letterbook.Workers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Letterbook.Web;

public class WebServerFixture
{
	private IHost? _host;
	public Uri BaseUrl { get; private set; } = Settings.BaseUrl;

	public async Task InitializeAsync(int? port = null)
	{
		BaseUrl = new Uri($"http://localhost:{port ?? GetRandomUnusedPort()}");

		CreateHost();

		if (_host != null)
		{
			await _host.StartAsync();
		}
	}

	private void CreateHost()
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.WriteTo.Console()
			.CreateBootstrapLogger();

		Log.Logger.Information($"Starting server at <{BaseUrl}>");

		/*

			Wow I can NOT make `UseStartUp` work so using inline definition based on `Program.cs`.

			@todo: Try `UseStartUp` again

		*/
		var builder = WebApplication.CreateBuilder();

		// Pre initialize Serilog
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			.Enrich.FromLogContext()
			.Enrich.WithSpan()
			.WriteTo.Console()
			.CreateBootstrapLogger();

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
		builder.Services.AddIdentity<Core.Models.Account, IdentityRole<Guid>>(identity => identity.ConfigureIdentity())
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
			.AddApplicationPart(Assembly.GetAssembly(typeof(Web.Program))!);
		builder.Services.AddAuthorization(options =>
		{
			options.AddWebAuthzPolicy();
		});

		builder.WebHost.UseUrls(BaseUrl.ToString());

		var app = builder.Build();

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

		app.UseWhen(context => !context.Request.Path.StartsWithSegments("/Identity"), applicationBuilder =>
		{
			applicationBuilder.UseMiddleware<ProfileIdentityMiddleware>();
		});

		app.MapRazorPages();

		_host = app;
	}

	public Task DisposeAsync()
	{
		if (_host != null)
		{
			_host.Dispose();
		}
		return Task.CompletedTask;
	}

	private static int GetRandomUnusedPort()
	{
		using var listener = new TcpListener(IPAddress.Any, 0);

		listener.Start();
		var port = ((IPEndPoint)listener.LocalEndpoint).Port;
		listener.Stop();

		return port;
	}

	private static bool LocalHostPortIsInUse(Uri url)
	{
		using var tc = new TcpClient();

		try
		{
			tc.Connect(IPAddress.Loopback, url.Port);
			return tc.Connected;
		}
		catch (SocketException)
		{
			return false;
		}
	}
}