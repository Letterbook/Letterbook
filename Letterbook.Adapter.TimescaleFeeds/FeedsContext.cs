using System.Reflection;
using Letterbook.Adapter.TimescaleFeeds.Entities;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Letterbook.Adapter.TimescaleFeeds;

public class FeedsContext : DbContext
{
    private FeedsDbOptions _feedsDbOptions;
    public DbSet<TimelineEntry> Feeds { get; set; }

    // Called by the designer to create and run migrations
    internal FeedsContext(DbContextOptions<FeedsContext> options) : base(options)
    {
        _feedsDbOptions = new FeedsDbOptions
        {
            Host = "localhost",
            Port = "5432",
            Username = "letterbook",
            Password = "letterbookpw",
            UseSsl = false,
            Database = "letterbook_feeds",
        };
    }

    // Called by DI during normal use
    public FeedsContext(IOptions<FeedsDbOptions> options)
    {
        _feedsDbOptions = options.Value;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_feedsDbOptions.GetConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
