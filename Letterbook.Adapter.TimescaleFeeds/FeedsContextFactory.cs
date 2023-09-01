using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Letterbook.Adapter.TimescaleFeeds;

/// <summary>
/// This class is only used by EFCore design tools, to generate migrations 
/// </summary>
public class FeedsContextFactory : IDesignTimeDbContextFactory<FeedsContext>
{
    public FeedsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FeedsContext>();

        return new FeedsContext(optionsBuilder.Options);
    }
}
