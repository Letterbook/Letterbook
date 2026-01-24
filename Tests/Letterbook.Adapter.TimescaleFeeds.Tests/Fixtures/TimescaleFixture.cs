using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Letterbook.Adapter.TimescaleFeeds._Tests.Fixtures;

public class TimescaleFixture<T>
{
	// @todo: duplicate of HostFixture.FeedsConnectionString
	private readonly string _connectionString =
		$"Server={FeedsConnectionStringHost()};" +
		"Port=5433;" +
		$"Database=letterbook_feeds_{typeof(T).Name};" +
		"User Id=letterbook;" +
		"Password=letterbookpw;" +
		"SSL Mode=Disable;" +
		"Search Path=public;" +
		"Include Error Detail=true";

	private static string FeedsConnectionStringHost()
		=> Environment.GetEnvironmentVariable(nameof(FeedsConnectionStringHost)) ?? "localhost";

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