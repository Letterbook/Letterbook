using System.Reflection;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Adapter.Db;

/// <summary>
/// This is the DbContext for most of the application. The actual data records (like the contents of profiles and
/// posts) will be managed here.
/// 
/// Feeds/timelines will likely be managed separately from this, likely using a timeseries db, probably timescale.
/// Maybe notifications, too.
/// There may be a need for search and/or graph databases in the future, and those would also be separate from this.
/// </summary>
public class TransactionalContext : DbContext
{
    private readonly ILogger<TransactionalContext> _logger;
    private readonly DbOptions _config;
    public DbSet<ApObject> Objects { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Audience> Audiences { get; set; }

    public TransactionalContext(DbContextOptions<TransactionalContext> context) : base(context)
    {
        _logger = new Logger<TransactionalContext>(new LoggerFactory());
        _config = new DesignDbOptions();
    }

    public TransactionalContext(ILogger<TransactionalContext> logger, IOptions<DbOptions> config)
    {
        _logger = logger;
        _config = config.Value;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_config.GetConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}