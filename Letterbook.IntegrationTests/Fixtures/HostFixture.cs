using Bogus;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Letterbook.Core;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Letterbook.Core.Workers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

public class HostFixture<T> : WebApplicationFactory<Program>
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
	private readonly IServiceScope _scope;

	public readonly CoreOptions Options;
	private NpgsqlDataSourceBuilder _ds;
	private readonly RelationalContext _context;
	private readonly FeedsContext _feedsContext;

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
		_feedsContext = CreateFeedsContext(_scope);

		InitializeTestData();
	}

	/// <summary>
	/// Create a new FeedsContext from the given scope
	/// Call <see cref="CreateScope"/> to get a new scope
	/// </summary>
	/// <param name="scope"></param>
	/// <returns></returns>
	public FeedsContext CreateFeedsContext(IServiceScope scope) =>
		scope.ServiceProvider.GetRequiredService<FeedsContext>();

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
		_sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed(T.Seed())));
		InitTestData();
		InitTimelineData();

		_context.Database.EnsureDeleted();
		_context.Database.Migrate();
		_context.Accounts.AddRange(Accounts);
		_context.Profiles.AddRange(Profiles);
		_context.SaveChanges();

		_context.Posts.AddRange(Posts.SelectMany(pair => pair.Value));
		_context.Posts.AddRange(Timeline);
		_context.SaveChanges();

		_feedsContext.Database.EnsureDeleted();
		_feedsContext.Database.Migrate();
		_feedsContext.AddRange(Timeline.Select(p => TimelinePost.Denormalize(p)).SelectMany(p => p));
		_feedsContext.SaveChanges();
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

				services.AddAuthentication("Test")
					.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
				services.AddAuthorization(options =>
				{
					options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
						.RequireAuthenticatedUser()
						.Build();
				});
				services.ConfigureApplicationCookie(options =>
				{
					options.ForwardAuthenticate = "Test";
				});
			});

		base.ConfigureWebHost(builder);
	}

	private void InitTestData()
	{
		var authority = Options.BaseUri() ?? new Uri("letterbook.example");
		Accounts.AddRange(new FakeAccount(false).Generate(2));
		Profiles.AddRange(new FakeProfile(authority, Accounts[0]).Generate(3));
		Profiles.Add(new FakeProfile(authority, Accounts[1]).Generate());
		Profiles.AddRange(new FakeProfile().Generate(3));

		// P0 follows P4 and P5
		// P4 follows P0
		Profiles[0].Follow(Profiles[4], FollowState.Accepted);
		Profiles[0].Follow(Profiles[5], FollowState.Accepted);
		Profiles[0].AddFollower(Profiles[4], FollowState.Accepted);
		// P1 has requested to follow P5
		Profiles[1].FollowingCollection.Add(new FollowerRelation(Profiles[1], Profiles[5], FollowState.Pending));

		// Local profiles
		// P0 creates posts 0-2
		Posts.Add(Profiles[0], new FakePost(Profiles[0]).Generate(3));
		// P0 creates post 3, as reply to 2
		Posts[Profiles[0]].Add(new FakePost(Profiles[0], Posts[Profiles[0]][2]).Generate());
		// P1 creates posts 0-7
		Posts.Add(Profiles[1], new FakePost(Profiles[0], opts: Options).Generate(8));
		// P2 creates post 0, and draft 1
		Posts.Add(Profiles[2], new FakePost(Profiles[2]).Generate(1));
		Posts[Profiles[2]].Add(new FakePost(Profiles[2], draft: true).Generate());

		// Remote profiles
		// P4 creates post 0, as reply to post P0:3
		Posts.Add(Profiles[4], new FakePost(Profiles[3], Posts[Profiles[0]][3]).Generate(1));

		var allAudience = Posts
			.SelectMany(pair => pair.Value)
			.SelectMany(post => post.Audience)
			.ToHashSet();

		foreach (var post in Posts.SelectMany(pair => pair.Value))
		{
			var set = new HashSet<Audience>();
			foreach (var audience in post.Audience)
			{
				set.Add(allAudience.TryGetValue(audience, out var existing) ? existing : audience);
			}

			post.Audience = set;
		}
	}

	private void InitTimelineData()
	{
		var authority = Options.BaseUri() ?? new Uri("letterbook.example");
		var faker = new Faker();

		while (Timeline.Count < T.TimelineCount())
		{
			Profile creator;
			// Pick or add new Creator
			if (Profiles.Count > 0 && faker.Random.Bool(0.9f))
			{
				creator = faker.PickRandom(Profiles);
			}
			else
			{
				if (faker.Random.Bool(0.3f))
				{
					creator = new FakeProfile(authority).Generate();
					Profiles.Add(creator);
				}
				else
				{
					creator = new FakeProfile().Generate();
					Profiles.Add(creator);
				}
			}

			Timeline.AddRange(GeneratePosts(creator));
		}

		var allAudience = Posts
			.SelectMany(pair => pair.Value)
			.SelectMany(post => post.Audience)
			.ToHashSet();
		allAudience.UnionWith(Timeline.SelectMany(post => post.Audience));

		foreach (var post in Timeline)
		{
			var set = new HashSet<Audience>(allAudience);
			set.IntersectWith(post.Audience);

			post.Audience = set;
		}

		return;

		IEnumerable<Post> GeneratePosts(Profile creator)
		{
			Post? inReplyTo = default;
			if (Timeline.Count != 0 && faker.Random.Bool(0.6f))
				inReplyTo = faker.PickRandom(Timeline);

			var fake = inReplyTo == null
				? new FakePost(creator)
				: new FakePost(creator, inReplyTo);

			var count = inReplyTo == null
				? faker.Random.Number(1, 3)
				: faker.Random.Number(1, 9);

			return fake.Generate(count);
		}
	}
}

public interface ITestSeed
{
	static virtual int? Seed() => null;
	static virtual int TimelineCount() => 0;
}