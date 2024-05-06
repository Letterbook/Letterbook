using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Letterbook.Adapter.TimescaleFeeds;

/// <summary>
/// This class is only used by EFCore design tools, to generate migrations
/// </summary>
public class FeedsContextFactory : IDesignTimeDbContextFactory<FeedsContext>
{
	public FeedsContext CreateDbContext(string[] args)
	{
		var feedsDbOptions = new FeedsDbOptions
		{
			Host = "localhost",
			Port = "5433",
			Username = "letterbook",
			Database = "letterbook_feeds",
			UseSsl = false,
			Password = "letterbookpw"
		};
		var optionsBuilder = new DbContextOptionsBuilder<FeedsContext>();

		return new FeedsContext(optionsBuilder
			.UseNpgsql(DependencyInjection.DataSource(feedsDbOptions))
			.Options);
	}
}