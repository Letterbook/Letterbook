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
		var dbOptions = config.GetSection(DbOptions.ConfigKey)
			.Get<DbOptions>() ?? throw new ArgumentNullException(DbOptions.ConfigKey);
		var dataSource = DataSource(dbOptions);

		return services.AddDbContext<RelationalContext>(options => options.UseNpgsql(dataSource))
			.AddScoped<IAccountProfileAdapter, AccountProfileAdapter>()
			.AddScoped<IActivityAdapter, ActivityAdapter>()
			.AddScoped<IPostAdapter, PostAdapter>();
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

	internal static NpgsqlDataSource DataSource(DbOptions dbOptions)
	{
		var dataSource = new NpgsqlDataSourceBuilder(dbOptions.GetConnectionString())
		{
			Name = dbOptions.Database ?? "letterbook"
		};
		dataSource.EnableDynamicJson();

		return dataSource.Build();
	}
}