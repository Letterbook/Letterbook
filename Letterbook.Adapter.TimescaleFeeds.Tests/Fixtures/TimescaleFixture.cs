using Microsoft.EntityFrameworkCore;

namespace Letterbook.Adapter.TimescaleFeeds._Tests.Fixtures;

public class TimescaleFixture<T>
{
	private readonly string _connectionString =
		"Server=localhost;" +
		"Port=5433;" +
		$"Database=letterbook_feeds_{typeof(T).Name};" +
		"User Id=letterbook;" +
		"Password=letterbookpw;" +
		"SSL Mode=Disable;" +
		"Search Path=public;" +
		"Include Error Detail=true";

	private readonly DbContextOptions<FeedsContext> _opts;

	public TimescaleFixture()
	{
		_opts = new DbContextOptionsBuilder<FeedsContext>()
			.UseNpgsql(DependencyInjection.DataSource(new FeedsDbOptions() { ConnectionString = _connectionString }))
			.Options;
	}

	public FeedsContext CreateContext() => new(_opts);
}