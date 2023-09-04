using System.Reflection;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
#pragma warning disable CS8618
// EntityFramework does the right thing

namespace Letterbook.Adapter.Db;

/// <summary>
/// This is the DbContext for most of the application. The actual data records (like the contents of profiles and
/// posts) will be managed here.
/// 
/// Feeds/timelines will likely be managed separately from this, likely using a timeseries db, probably timescale.
/// Maybe notifications, too.
/// There may be a need for search and/or graph databases in the future, and those would also be separate from this.
/// </summary>
public class RelationalContext : DbContext
{
    private readonly DbOptions _config;
    public DbSet<Note> Notes { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Account> Accounts { get; set; }

    // Called by the designer to create and run migrations
    internal RelationalContext(DbContextOptions<RelationalContext> context) : base(context)
    {
        _config = new DesignDbOptions();
    }

    // Called by DI for normal use
    public RelationalContext(IOptions<DbOptions> config)
    {
        _config = config.Value;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_config.GetConnectionString());
        options.UseOpenIddict();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}