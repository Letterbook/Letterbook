using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit.Abstractions;
using Xunit.Sdk;
using RelationalContext = Letterbook.Adapter.Db.RelationalContext;

namespace Letterbook.IntegrationTests.Fixtures;

public class HostFixture<T> : WebApplicationFactory<Program>, IIntegrationTestData
where T : ITestSeed
{
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_scope.Dispose();
		}

		base.Dispose(disposing);
	}

	private readonly IMessageSink _sink;
	private static readonly object _lock = new();

	private string ConnectionString =
		$"Database=letterbook_{typeof(T).Name};" +
		"Server=localhost;" +
		"Port=5432;" +
		"User Id=letterbook;" +
		"Password=letterbookpw;" +
		"SSL Mode=Disable;" +
		"Search Path=public;" +
		"Include Error Detail=true";
	private string FeedsConnectionString =
		$"Database=letterbook_feeds_{typeof(T).Name};" +
		"Server=localhost;" +
		"Port=5433;" +
		"User Id=letterbook;" +
		"Password=letterbookpw;" +
		"SSL Mode=Disable;" +
		"Search Path=public;" +
		"Include Error Detail=true";

	public List<Profile> Profiles { get; set; } = new();
	public List<Account> Accounts { get; set; } = new();
	public Dictionary<Profile, List<Post>> Posts { get; set; } = new();
	public List<Post> Timeline { get; set; } = new();
	public int Records { get; set; }
	public bool Deleted { get; set; }
	private readonly IServiceScope _scope;

	public readonly CoreOptions Options;
	private NpgsqlDataSourceBuilder _ds;
	private RelationalContext _context;

	public HostFixture(IMessageSink sink)
	{
		_sink = sink;
		Options = new CoreOptions
		{
			DomainName = "localhost",
			Scheme = "http",
			Port = "5127"
		};

		_ds = new NpgsqlDataSourceBuilder(ConnectionString);
		_ds.EnableDynamicJson();
		_scope = CreateScope();
		_context = CreateContext(_scope);

		InitializeTestData();
	}

	/// <summary>
	/// Create a new RelationalContext from the given scope
	/// Call <see cref="CreateScope"/> to get a new scope
	/// </summary>
	/// <param name="scope"></param>
	/// <returns></returns>
	public RelationalContext CreateContext(IServiceScope scope) =>
		scope.ServiceProvider.GetRequiredService<RelationalContext>();

	/// <summary>
	/// Create a new scope for resolving scoped services
	/// Be sure to dispose the scope when you're done
	/// </summary>
	/// <returns></returns>
	public IServiceScope CreateScope() => Services.CreateScope();

	private void InitializeTestData()
	{
		lock (_lock)
		{

			_sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed(T.Seed())));
			this.InitTestData(Options);
			// this.InitTimelineData(Options);

			Deleted = _context.Database.EnsureDeleted();
			_context.Database.Migrate();
			_context.Accounts.AddRange(Accounts);
			_context.Profiles.AddRange(Profiles);
			Records = _context.SaveChanges();
			_context.Posts.AddRange(Posts.SelectMany(pair => pair.Value));
			Records += _context.SaveChanges();
			// context.Posts.AddRange(Timeline);
			// Records += context.SaveChanges();
		}
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Override the db connection strings (and potentially other config) so that we can have a db-per-class
		// This eliminates a huge source of test data conflicts, and should make them parallelizable
		// See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#configuration-keys-and-values
		var memConfig = new Dictionary<string, string>()
		{
			[$"{DbOptions.ConfigKey}:{nameof(DbOptions.ConnectionString)}"] = ConnectionString,
			[$"{FeedsDbOptions.ConfigKey}:{nameof(FeedsDbOptions.ConnectionString)}"] = FeedsConnectionString
		};
		var cfg = new ConfigurationBuilder().AddInMemoryCollection(memConfig!).Build();

		builder
			.UseConfiguration(cfg)
			.ConfigureServices(services =>
			{
				// SeedAdminWorker executes before we have a chance to create the test database
				// So we just remove it
				var seedDescriptor = services.SingleOrDefault(d => d.ImplementationType == typeof(WorkerScope<SeedAdminWorker>));

				if (seedDescriptor != null) services.Remove(seedDescriptor);
				// services.AddSingleton<RelationalContext>();
				// services.AddSingleton<FeedsContext>();
			});

		base.ConfigureWebHost(builder);
	}
}

public interface ITestSeed
{
	static abstract int? Seed();
}