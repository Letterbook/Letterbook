using Letterbook.Core.Models;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Letterbook.Adapter.Db.IntegrationTests.Fixtures;

public class PostgresFixture : IIntegrationTestData
{
    private const string ConnectionString =
        @"Server=localhost;Port=5432;Database=letterbook_tests;User Id=letterbook;Password=letterbookpw;SSL Mode=Disable;Search Path=public;Include Error Detail=true";
    private static readonly object _lock = new();

    private IOptions<DbOptions> _opts = Options.Create(new DbOptions { ConnectionString = ConnectionString });
    public RelationalContext CreateContext() => new RelationalContext(_opts);
    public List<Profile> Profiles { get; set; } = new();
    public List<Account> Accounts { get; set; } = new();
    public Dictionary<Profile, List<Post>> Posts { get; set; } = new();
    public int Records;
    public bool Deleted;

    public PostgresFixture(IMessageSink sink)
    {
        lock (_lock)
        {
			sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));
			this.InitTestData();

            using (RelationalContext context = CreateContext())
            {
                Deleted = context.Database.EnsureDeleted();
                context.Database.Migrate();
                context.AddRange(Accounts);
                context.AddRange(Profiles);
                Records = context.SaveChanges();
                context.AddRange(Posts.SelectMany(pair => pair.Value));
                Records += context.SaveChanges();
            }
        }
    }
}