using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Letterbook.Adapter.TimescaleFeeds;

public static class DependencyInjection
{
	public static IServiceCollection AddFeedsAdapter(this IServiceCollection services, IConfigurationSection config)
	{
		var dbOptions = config.Get<FeedsDbOptions>() ?? throw new ArgumentNullException(FeedsDbOptions.ConfigKey);
		var dataSource = DataSource(dbOptions);

		return services.AddDbContext<FeedsContext>(options => options.UseNpgsql(dataSource));
	}

	internal static NpgsqlDataSource DataSource(FeedsDbOptions dbOptions)
	{
		var dataSource = new NpgsqlDataSourceBuilder(dbOptions.GetConnectionString())
		{
			Name = dbOptions.Database ?? "letterbook_feeds"
		};
		// dataSource.EnableDynamicJson();

		return dataSource.Build();
	}
}