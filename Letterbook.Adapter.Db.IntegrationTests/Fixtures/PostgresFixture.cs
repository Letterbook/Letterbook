using Letterbook.Core.Models;
using Letterbook.Core.Extensions;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Letterbook.Adapter.Db.IntegrationTests.Fixtures;

public class PostgresFixture
{
    private const string ConnectionString =
        @"Server=localhost;Port=5432;Database=letterbook_tests;User Id=letterbook;Password=letterbookpw;SSL Mode=Disable;Search Path=public";
    private static readonly object _lock = new();
    private static bool _databaseInitialized = false;

    private IOptions<DbOptions> _opts = Options.Create(new DbOptions { ConnectionString = ConnectionString });
    public RelationalContext CreateContext() => new RelationalContext(_opts);
    public List<Profile> Profiles = new List<Profile>();
    public List<Account> Accounts = new List<Account>();

    public PostgresFixture(IMessageSink sink)
    {
        sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));
        lock (_lock)
        {
            if (_databaseInitialized) return;

            Accounts.AddRange(new FakeAccount(false).Generate(2));
            Profiles.AddRange(new FakeProfile("letterbook.example", Accounts[0]).Generate(3));
            Profiles.Add(new FakeProfile("letterbook.example", Accounts[1]).Generate());
            Profiles.AddRange(new FakeProfile().Generate(3));
            
            // Profiles[0]
            Profiles[0].Follow(Profiles[4], FollowState.Accepted);
            Profiles[0].Follow(Profiles[5], FollowState.Accepted);
            Profiles[0].AddFollower(Profiles[4], FollowState.Accepted);

            using (RelationalContext context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
                context.AddRange(Accounts);
                // context.SaveChanges();
                context.AddRange(Profiles);
                context.SaveChanges();
            }
            _databaseInitialized = true;
        }
    }

}