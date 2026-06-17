using Letterbook.Adapter.ActivityPub;
using Letterbook.Core.Adapters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;

namespace Letterbook.Adapter.Db;

public static class DependencyInjection
{
	public static IServiceCollection AddDbAdapter(this IServiceCollection services, IConfigurationManager config)
	{
		var dataSource = DataSource(config);
		return services.AddDbContext<RelationalContext>(options =>
			{
				options.UseNpgsql(dataSource);
				options.UseProjectables();
			})
			.AddScoped<IDataAdapter, DataAdapter>()
			.AddScoped<ISearchProvider, DataAdapter>()
			.AddKeyedScoped<ISearchProvider, DataAdapter>("primary")
			.AddKeyedScoped<ISearchProvider, WebFingerClient>("secondary")
			.AddScoped<ISearchProvider, FallbackSearchProvider>(
				s => new FallbackSearchProvider(
					s.GetKeyedService<ISearchProvider>("primary")!,
					s.GetKeyedService<ISearchProvider>("secondary")!)
			);
	}

	public static IOpenTelemetryBuilder AddDbTelemetry(this IOpenTelemetryBuilder builder)
	{
		return builder.WithMetrics(metrics =>
			{
				metrics.AddMeter("Npgsql");
			})
			.WithTracing(tracing =>
			{
				tracing.AddNpgsql();
			});
	}

	internal static NpgsqlDataSource DataSource(IConfiguration config)
	{
		var dataSource = new NpgsqlDataSourceBuilder(config.GetConnectionString("db"));
		dataSource.EnableDynamicJson();

		return dataSource.Build();
	}
}