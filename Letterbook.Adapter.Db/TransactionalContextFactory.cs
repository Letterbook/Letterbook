using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Letterbook.Adapter.Db;

/// <summary>
/// This class is only used by EFCore design tools, to generate migrations 
/// </summary>
public class TransactionalContextFactory : IDesignTimeDbContextFactory<TransactionalContext>
{
    public TransactionalContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TransactionalContext>();

        return new TransactionalContext(optionsBuilder.Options);
    }
}
