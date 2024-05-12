using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Hosting;
using OpenTelemetry.Resources;

namespace Letterbook.Workers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLetterbookWorkers(builder.Configuration.GetSection(WorkerOptions.ConfigKey))
	        .AddLetterbookCore(builder.Configuration.GetSection(CoreOptions.ConfigKey))
	        .AddDbAdapter(builder.Configuration.GetSection(DbOptions.ConfigKey))
	        .AddFeedsAdapter(builder.Configuration.GetSection(FeedsDbOptions.ConfigKey))
	        .AddActivityPubClient(builder.Configuration)
	        .AddHealthChecks();

        builder.Services.AddIdentity();

        builder.Services.AddOpenTelemetry()
	        .ConfigureResource(resource => { resource.AddService( "Letterbook.Workers"); })
	        .AddWorkerTelemetry()
	        .AddClientTelemetry()
	        .AddDbTelemetry()
	        .AddTelemetryExporters();

        var app = builder.Build();

        app.Run();
    }
}
