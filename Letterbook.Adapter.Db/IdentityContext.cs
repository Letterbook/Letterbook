using System.Reflection;
using Letterbook.Adapter.Db.EntityConfigs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
#pragma warning disable CS8618

namespace Letterbook.Adapter.Db;

// public class IdentityContext : IdentityDbContext
// {
//     private readonly DbOptions _config;
//     public DbSet<Models.Account> Accounts { get; set; }
//     
//     // Called by the designer to create and run migrations
//     internal IdentityContext(DbContextOptions<IdentityContext> context) : base(context)
//     {
//         _config = new DesignDbOptions();
//     }
//
//     // Called by DI for normal use
//     public IdentityContext(IOptions<DbOptions> config)
//     {
//         _config = config.Value;
//     }
//
//
//     protected override void OnConfiguring(DbContextOptionsBuilder options)
//     {
//         options.UseNpgsql(_config.GetConnectionString());
//     }
//
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         base.OnModelCreating(modelBuilder);
//         // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
//         modelBuilder.Entity<Models.Account>().ToTable(e => e.ExcludeFromMigrations());
//         modelBuilder.Entity<Models.Profile>().ToTable("Profiles", builder => builder.ExcludeFromMigrations());
//         modelBuilder.ApplyConfiguration(new ConfigureLinkedProfile());
//     }
// }