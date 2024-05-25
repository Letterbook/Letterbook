using Letterbook.Adapter.Db;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Xunit.Abstractions;
using Xunit.Sdk;
using RelationalContext = Letterbook.Adapter.Db.RelationalContext;

namespace Letterbook.Api.IntegrationTests.Fixtures;

public class HostFixture<T> : WebApplicationFactory<Program>, IIntegrationTestData
{
	private readonly IMessageSink _sink;
	private static readonly object _lock = new();

	private string ConnectionString =
		$"Database=letterbook_{typeof(T).Name};Server=localhost;Port=5432;User Id=letterbook;Password=letterbookpw;SSL Mode=Disable;Search Path=public;Include Error Detail=true";

	public List<Profile> Profiles { get; set; } = new();
	public List<Account> Accounts { get; set; } = new();
	public Dictionary<Profile, List<Post>> Posts { get; set; } = new();
	public int Records { get; set; }
	public bool Deleted { get; set; }

	public readonly CoreOptions Options;
	public readonly DbOptions DbOptions;

	public HostFixture(IMessageSink sink)
	{
		_sink = sink;
		Options = new CoreOptions
		{
			DomainName = "localhost",
			Scheme = "http",
			Port = "5127"
		};

		InitializeTestData();
	}

	private void InitializeTestData()
	{
		lock (_lock)
		{
			_sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));
			this.InitTestData(Options);

			// using var scope = services.CreateScope();
			// var context = scope.ServiceProvider.GetRequiredService<RelationalContext>();
			var ds = new NpgsqlDataSourceBuilder(ConnectionString);
			ds.EnableDynamicJson();
			var context = new RelationalContext(new DbContextOptionsBuilder<RelationalContext>()
				.UseNpgsql(ds.Build())
				.Options);

			Deleted = context.Database.EnsureDeleted();
			context.Database.Migrate();
			context.Accounts.AddRange(Accounts);
			context.Profiles.AddRange(Profiles);
			Records = context.SaveChanges();
			context.Posts.AddRange(Posts.SelectMany(pair => pair.Value));
			Records += context.SaveChanges();
		}
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Override the db connection string (and potentially other config) so that we can have a db-per-class
		// This eliminates a huge source of test data conflicts, and should make them parallelizable
		// See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#configuration-keys-and-values
		var memConfig = new Dictionary<string, string>()
		{
			[DbOptions.ConfigKey + $":{nameof(DbOptions.ConnectionString)}"] = ConnectionString
		};
		var cfg = new ConfigurationBuilder().AddInMemoryCollection(memConfig).Build();

		builder
			.UseConfiguration(cfg)
			.ConfigureServices(services =>
			{
				// SeedAdminWorker executes before we have a chance to create the test database
				// So we just remove it
				var seedDescriptor = services.SingleOrDefault(d => d.ImplementationType == typeof(WorkerScope<SeedAdminWorker>));

				services.Remove(seedDescriptor);
			});

		base.ConfigureWebHost(builder);
	}
}