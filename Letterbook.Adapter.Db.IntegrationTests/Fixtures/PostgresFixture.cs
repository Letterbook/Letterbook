using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Letterbook.Adapter.Db.IntegrationTests.Fixtures;

public class PostgresFixture
{
    private const string ConnectionString =
        @"Server=localhost;Port=5432;Database=letterbook_feeds;User Id=letterbook;Password=letterbookpw;SSL Mode=Disable;Search Path=public";
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    private IOptions<DbOptions> _opts = Options.Create(new DbOptions { ConnectionString = ConnectionString });
    public RelationalContext CreateContext() => new RelationalContext(_opts);

    public PostgresFixture()
    {
        lock (_lock)
        {
            if (_databaseInitialized) return;
            using (RelationalContext context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }

            _databaseInitialized = true;
        }
    }

}