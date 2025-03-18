using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Letterbook.Adapter.TimescaleFeeds;

/// <summary>
/// This class is only used by EFCore design tools, to generate migrations
/// </summary>
public class FeedsContextFactory : IDesignTimeDbContextFactory<FeedsContext>
{
	public FeedsContext CreateDbContext(string[] args)
	{
		var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
		              ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
		Console.WriteLine($"Looking for appsettings files at {AppDomain.CurrentDomain.BaseDirectory}");
		var configBuilder = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", true, false)
			.AddJsonFile($"appsettings.{envName}.json", true, false);

		var config = configBuilder.Build();
		var optionsBuilder = new DbContextOptionsBuilder<FeedsContext>();

		return new FeedsContext(optionsBuilder
			.UseNpgsql(DependencyInjection.DataSource(config))
			.Options);
	}
}