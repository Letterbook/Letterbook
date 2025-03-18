using Microsoft.EntityFrameworkCore;
using Npgsql;

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
		var dataSource = new NpgsqlDataSourceBuilder(_connectionString);
		dataSource.EnableDynamicJson();

		_opts = new DbContextOptionsBuilder<FeedsContext>()
			.UseNpgsql(dataSource.Build())
			.Options;
	}

	public FeedsContext CreateContext() => new(_opts);
}