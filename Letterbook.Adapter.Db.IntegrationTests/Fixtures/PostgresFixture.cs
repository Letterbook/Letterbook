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
        @"Server=localhost;Port=5432;Database=letterbook_tests;User Id=letterbook;Password=letterbookpw;SSL Mode=Disable;Search Path=public;Include Error Detail=true";
    private static readonly object _lock = new();

    private IOptions<DbOptions> _opts = Options.Create(new DbOptions { ConnectionString = ConnectionString });
    public RelationalContext CreateContext() => new RelationalContext(_opts);
    public List<Profile> Profiles = new();
    public List<Account> Accounts = new();
    public Dictionary<Profile, List<Post>> Posts = new();
    public int Records;
    public bool Deleted;

    public PostgresFixture(IMessageSink sink)
    {
        lock (_lock)
        {
			sink.OnMessage(new DiagnosticMessage("Bogus Seed: {0}", Init.WithSeed()));

            Accounts.AddRange(new FakeAccount(false).Generate(2));
            Profiles.AddRange(new FakeProfile("letterbook.example", Accounts[0]).Generate(3));
            Profiles.Add(new FakeProfile("letterbook.example", Accounts[1]).Generate());
            Profiles.AddRange(new FakeProfile().Generate(3));

            // P0 follows P4 and P5
            // P4 follows P0
            Profiles[0].Follow(Profiles[4], FollowState.Accepted);
            Profiles[0].Follow(Profiles[5], FollowState.Accepted);
            Profiles[0].AddFollower(Profiles[4], FollowState.Accepted);

            // P0 creates posts 0-2
            Posts.Add(Profiles[0], new FakePost(Profiles[0]).Generate(3));
            // P0 creates post 3, as reply to 2
            Posts[Profiles[0]].Add(new FakePost(Profiles[0], Posts[Profiles[0]][2]).Generate());
            // P2 creates post 0
            Posts.Add(Profiles[2], new FakePost(Profiles[2]).Generate(1));
            // P4 creates post 0, as reply to P0:3
            Posts.Add(Profiles[4], new FakePost(Profiles[3], Posts[Profiles[0]][3]).Generate(1));

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