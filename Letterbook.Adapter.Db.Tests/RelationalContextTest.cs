using Letterbook.Core.Tests.Fakes;
using Microsoft.Extensions.Options;

namespace Letterbook.Adapter.Db.Tests;

public class RelationalContextTest
{
    private readonly RelationalContext _context;

    public RelationalContextTest()
    {
        _context = new RelationalContext(Options.Create(new DbOptions
        {
            Host = "localhost",
            Port = "5432",
            Username = "postgres",
            Database = "postgres",
            UseSsl = false,
            Password = "postgres",
        }));
    }
    
    [Fact]
    public void CanAddEntity()
    {
        var account = new FakeAccount().Generate();
        _context.Accounts.Add(account);
    }
}