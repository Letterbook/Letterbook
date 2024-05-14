using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Config;
using Letterbook.Core.Extensions;
using MassTransit;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Letterbook.Workers
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = Host.CreateApplicationBuilder(args);

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.ReadFrom.Configuration(builder.Configuration)
				.Enrich.FromLogContext()
				.Enrich.WithSpan()
				.WriteTo.Console()
				.CreateBootstrapLogger();

			builder.Services
				.AddSerilog(s => s.Enrich.FromLogContext()
					.Enrich.WithSpan()
					.ReadFrom.Configuration(builder.Configuration))
				.AddMassTransit(bus => bus.AddWorkers(builder.Configuration)
					.AddWorkerBus(builder.Configuration))
				.AddLetterbookCore(builder.Configuration)
				.AddDbAdapter(builder.Configuration)
				.AddFeedsAdapter(builder.Configuration)
				.AddActivityPubClient(builder.Configuration);

			builder.Services.AddIdentity();

			builder.Services.AddOpenTelemetry()
				.ConfigureResource(resource => { resource.AddService("Letterbook.Workers"); })
				.AddWorkerTelemetry()
				.AddClientTelemetry()
				.AddDbTelemetry()
				.AddTelemetryExporters();

			await builder.Build().RunAsync();
		}
	}
}