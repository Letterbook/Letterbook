using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Letterbook.Adapter.Db;

/// <summary>
/// This class is only used by EFCore design tools, to generate migrations 
/// </summary>
public class RelationalContextFactory : IDesignTimeDbContextFactory<RelationalContext>
{
    public RelationalContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RelationalContext>();

        return new RelationalContext(optionsBuilder.Options);
    }
}
