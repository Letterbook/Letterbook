using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

		// Suppress warning about too many EFCore service providers created
		// The warning refers to the app domain, which for us is the entire integration tests project
		// It's expected that we will create quite a few due to the way we isolate test data
		// services
		_opts = new DbContextOptionsBuilder<FeedsContext>()
			.ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
			.UseNpgsql(dataSource.Build())
			.Options;
	}

	public FeedsContext CreateContext() => new(_opts);
}