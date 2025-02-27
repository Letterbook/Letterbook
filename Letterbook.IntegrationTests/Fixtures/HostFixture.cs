using System.Diagnostics;
using System.Net;
using ActivityPub.Types.AS;
using Bogus;
using Letterbook.Adapter.Db;
using Letterbook.Adapter.TimescaleFeeds;
using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Letterbook.Api.Authentication.HttpSignature.Handler;
using Letterbook.AspNet.Tests.Fixtures;
using Letterbook.Core;
using Letterbook.Core.Adapters;
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
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using OpenTelemetry.Trace;
using Xunit.Abstractions;
using Xunit.Sdk;
using RelationalContext = Letterbook.Adapter.Db.RelationalContext;

namespace Letterbook.IntegrationTests.Fixtures;

/// <summary>
/// A Class fixture to manage the application under test, and its test data, with isolation per-test class
/// </summary>
/// <remarks>
/// Reminder: do not dispose this object. It will be disposed by XUnit, after the test class has finished running.
/// Disposing early will break other tests, because it will shut down the application.
/// </remarks>
/// <typeparam name="T"></typeparam>
public class HostFixture<T> : WebApplicationFactory<Program>
	where T : ITestSeed
{
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_scope.Dispose();
			_context.Dispose();
			_feedsContext.Dispose();
			_spans.Dispose();
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
	public List<ModerationReport> Reports { get; set; } = new();
	public List<ModerationPolicy> Policies { get; set; } = new();
	public Dictionary<Profile, List<Post>> Posts { get; set; } = new();
	public List<Post> Timeline { get; set; } = new();
	public IAsyncEnumerable<Activity> Spans => _spans.ToAsyncEnumerable();
	public Mock<IActivityPubClient> MockActivityPubClient = new();
	public Mock<IActivityPubAuthenticatedClient> MockActivityPubClientAuth = new();
	public WebApplicationFactoryClientOptions DefaultOptions => new()
	{
		BaseAddress = Options?.BaseUri() ?? new Uri("localhost:5127"),
		AllowAutoRedirect = false
	};

	private readonly IServiceScope _scope;

	public readonly CoreOptions Options;
	private NpgsqlDataSourceBuilder _ds;
	private readonly RelationalContext _context;
	private readonly FeedsContext _feedsContext;
	private readonly CollectionSubject<Activity> _spans = new();

	public HostFixture(IMessageSink sink)
	{
		MockActivityPubClient.Setup(m => m.As(It.IsAny<IFederatedActor>()))
			.Returns(MockActivityPubClientAuth.Object);
		MockActivityPubClientAuth.Setup(m => m.SendDocument(It.IsAny<Uri>(), It.IsAny<string>()))
			.ReturnsAsync(new ClientResponse<object>
			{
				Data = null,
				DeliveredAddress = null,
				StatusCode = HttpStatusCode.Accepted
			});
		MockActivityPubClientAuth.Setup(m => m.SendDocument(It.IsAny<Uri>(), It.IsAny<ASType>()))
			.ReturnsAsync(new ClientResponse<object>
			{
				Data = null,
				DeliveredAddress = null,
				StatusCode = HttpStatusCode.Accepted
			});
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
		DataCleanupRefs();

		_context.Database.EnsureDeleted();
		_context.Database.Migrate();
		_context.Accounts.AddRange(Accounts);
		_context.Profiles.AddRange(Profiles);
		_context.Profiles.Add(Profile.GetOrAddInstanceProfile(Options));
		_context.Profiles.Add(Profile.GetOrAddModeratorsProfile(Options));
		_context.SaveChanges();

		_context.Posts.AddRange(Posts.SelectMany(pair => pair.Value));
		_context.SaveChanges();

		_context.ModerationPolicy.AddRange(Policies);
		_context.ModerationReport.AddRange(Reports);
		_context.SaveChanges();

		_feedsContext.Database.EnsureDeleted();
		_feedsContext.Database.Migrate();
		_feedsContext.AddRange(Timeline.Select(p => TimelinePost.Denormalize(p)).SelectMany(p => p));
		_feedsContext.SaveChanges();
	}

	private void DataCleanupRefs()
	{
		var allAudience = Profiles.SelectMany(profile => profile.Headlining).ToHashSet();
		allAudience.UnionWith(Profiles.SelectMany(profile => profile.Audiences));
		allAudience.UnionWith(Posts.SelectMany(pair => pair.Value).SelectMany(post => post.Audience));

		foreach (var p in Profiles)
		{
			p.Audiences = p.Audiences.ReplaceFrom(allAudience);
			p.Headlining = p.Headlining.ReplaceFrom(allAudience);
		}

		foreach (var post in Timeline)
		{
			post.Audience = post.Audience.ReplaceFrom(allAudience);
		}

		foreach (var post in Posts.SelectMany(pair => pair.Value))
		{
			post.Audience = post.Audience.ReplaceFrom(allAudience);
		}
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Override the db connection strings (and potentially other config) so that we can have a db-per-class
		// This eliminates a huge source of test data conflicts, and should make them parallelizable
		// See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-8.0#configuration-keys-and-values
		var memConfig = new Dictionary<string, string>()
		{
			["ConnectionStrings:db"] = ConnectionString,
			["ConnectionStrings:feeds"] = FeedsConnectionString,
			// [$"{DbOptions.ConfigKey}:{nameof(DbOptions.ConnectionString)}"] = ConnectionString,
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
					.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { })
					.AddScheme<HttpSignatureAuthenticationOptions, SignatureAuthHandler>(Api.Constants.ActivityPubPolicy, _ => { });
				services.AddAuthorization(options =>
				{
					options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
						.RequireAuthenticatedUser()
						.Build();
					options.AddPolicy(Api.Constants.ActivityPubPolicy, new AuthorizationPolicyBuilder(Api.Constants.ActivityPubPolicy)
						.RequireAuthenticatedUser()
						.Build());
				});
				services.ConfigureApplicationCookie(options =>
				{
					options.ForwardAuthenticate = "Test";
				});
				services.AddOpenTelemetry()
					.WithTracing(tracer =>
					{
						tracer.AddInMemoryExporter(_spans);
					});
				// We mock the IActivityPubClient because setting up and managing test data across federation peers is way beyond the scope
				// of what we can do. So we fake that part of it.
				services.AddScoped<IActivityPubClient>(_ => MockActivityPubClient.Object);
			});

		base.ConfigureWebHost(builder);
	}

	private void InitTestData()
	{
		var generic = new Faker();
		var authority = Options.BaseUri() ?? new Uri("letterbook.example");
		var peer = "peer.example";
		Accounts.AddRange(new FakeAccount(false).Generate(2));
		Accounts.Add(new FakeAccount().Generate());
		Profiles.AddRange(new FakeProfile(authority, Accounts[0]).Generate(3)); // P0-2
		Profiles.Add(new FakeProfile(authority, Accounts[1]).Generate()); // P3
		Profiles.AddRange(new FakeProfile().Generate(3)); // P4-6
		Profiles.AddRange(new FakeProfile(authority).Generate(3)); // P7-9 (Group: Follow)
		Profiles.AddRange(new FakeProfile(peer).Generate(4)); // P10-13 (Group: remote followers)

		// P0 follows P4 and P5
		// P4 follows P0
		Follow(Profiles[0], Profiles[4]);
		Follow(Profiles[0], Profiles[5]);
		Follow(Profiles[4], Profiles[0]);
		// P1 has requested to follow P5
		Profiles[1].FollowingCollection.Add(new FollowerRelation(Profiles[1], Profiles[5], FollowState.Pending));
		// P9 follows P8 and P7
		Follow(Profiles[0], Profiles[8]);
		Follow(Profiles[0], Profiles[7]);
		// P8 follows P9 (local) and P13 (remote)
		Follow(Profiles[8], Profiles[9]);
		Follow(Profiles[8], Profiles[13]);
		// P10-12 (remote peers) follow P7
		Follow(Profiles[10], Profiles[7]);
		Follow(Profiles[11], Profiles[7]);
		Follow(Profiles[12], Profiles[7]);

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
		// P8 creates post 0
		Posts.Add(Profiles[8], new FakePost(Profiles[8]).Generate(1));
		// P13 creates posts 0-1
		Posts.Add(Profiles[13], new FakePost(Profiles[13]).Generate(2));

		// Remote profiles
		// P4 creates post 0, as reply to post P0:3
		Posts.Add(Profiles[4], new FakePost(Profiles[3], Posts[Profiles[0]][3]).Generate(1));

		// Policies
		Policies.AddRange([
			// Policy0, retired
			new ModerationPolicy
			{
				Title = generic.Lorem.Sentence(),
				Summary = generic.Lorem.Sentence(6),
				Policy = generic.Lorem.Sentence(60),
				// retired 1-3 days ago
				Retired = generic.Date.BetweenOffset(DateTimeOffset.UtcNow - TimeSpan.FromDays(3), DateTimeOffset.UtcNow- TimeSpan.FromDays(1))
			},
			new ModerationPolicy
			{
				Title = generic.Lorem.Sentence(),
				Summary = generic.Lorem.Sentence(6),
				Policy = generic.Lorem.Sentence(60),
			},
			new ModerationPolicy
			{
				Title = generic.Lorem.Sentence(),
				Summary = generic.Lorem.Sentence(6),
				Policy = generic.Lorem.Sentence(60),
			}
		]);

		// Reports
		// P3 reports P5, linked to Policy1
		Reports.Add(new FakeReport(Profiles[3], Profiles[5]).Generate());
		Reports[0].Policies.Add(Policies[1]);
		// P3 reports P1+post7
		Reports.Add(new FakeReport(Profiles[3], Posts[Profiles[1]][7]).Generate());
		// P0 reports P1+post7
		Reports.Add(new FakeReport(Profiles[0], Posts[Profiles[1]][7]).Generate());
		// A2 assigned to R0-1
		foreach (var report in Reports.Take(2))
		{
			report.Moderators.Add(Accounts[2]);
		}

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

	private static void Follow(Profile self, Profile target)
	{
		self.Follow(target, FollowState.Accepted);
		self.Audiences.Add(target.Headlining.First(a => a == Audience.Followers(target)));
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