using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Letterbook.Adapter.Db;

public static class DependencyInjection
{
	public static IServiceCollection AddDbAdapter(this IServiceCollection services, IConfigurationSection config)
	{

		var dbOptions = config.Get<DbOptions>() ?? throw new ArgumentNullException(DbOptions.ConfigKey);

		return services.AddDbContext<RelationalContext>(options => options.UseNpgsql(DataSource(dbOptions)));
	}

	internal static NpgsqlDataSource DataSource(DbOptions dbOptions)
	{
		var dataSource = new NpgsqlDataSourceBuilder(dbOptions.GetConnectionString());
		dataSource.EnableDynamicJson();

		return dataSource.Build();
	}
}