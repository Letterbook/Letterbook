using Letterbook.Adapter.Db;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Letterbook.Api.IntegrationTests.Fixtures;

public class HostFixture : WebApplicationFactory<Program>, IIntegrationTestData
{
	private readonly IMessageSink _sink;
	private static readonly object _lock = new();

	public List<Profile> Profiles { get; set; } = new();
	public List<Account> Accounts { get; set; } = new();
	public Dictionary<Profile, List<Post>> Posts { get; set; } = new();
	public int Records { get; set; }
	public bool Deleted { get; set; }
	public CoreOptions? Options;

	public HostFixture(IMessageSink sink)
	{
		_sink = sink;

		lock (_lock)
		{
			_sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));
			Options = Services.GetRequiredService<IOptions<CoreOptions>>()?.Value;
			this.InitTestData(Options);

			using (var scope = Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<RelationalContext>();

				Deleted = context.Database.EnsureDeleted();
				context.Database.Migrate();
				context.AddRange(Accounts);
				context.AddRange(Profiles);
				Records = context.SaveChanges();
				context.AddRange(Posts.SelectMany(pair => pair.Value));
				Records += context.SaveChanges();
			}

			_sink.OnMessage(new DiagnosticMessage($"Saved {Records} records"));
		}
	}
}