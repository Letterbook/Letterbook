using System.Reflection;
using Letterbook.Adapter.Db.EntityConfigs;
using Letterbook.Core.Models;
using Medo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

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
    public DbSet<Note> Notes { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<ThreadContext> Threads { get; set; }

    // Called by the designer to create and run migrations
    internal RelationalContext(DbContextOptions<RelationalContext> context) : base(context)
    {
    }

    // Called by DI for normal use
    public RelationalContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Uri>().HaveConversion<UriIdConverter, UriIdComparer>();
        // configurationBuilder.Properties<Uuid7>().HaveConversion<UuidConverter>();
    }
}