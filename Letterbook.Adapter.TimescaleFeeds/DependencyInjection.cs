using Letterbook.Core.Adapters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Letterbook.Adapter.TimescaleFeeds;

public static class DependencyInjection
{
	public static IServiceCollection AddFeedsAdapter(this IServiceCollection services, IConfigurationManager config)
	{
		var dataSource = DataSource(config);
		return services.AddDbContext<FeedsContext>(options => options.UseNpgsql(dataSource))
			.AddScoped<IFeedsAdapter, FeedsAdapter>();
	}

	internal static NpgsqlDataSource DataSource(IConfiguration config)
	{
		var dataSource = new NpgsqlDataSourceBuilder(config.GetConnectionString("feeds"));
		dataSource.EnableDynamicJson();

		return dataSource.Build();
	}
}