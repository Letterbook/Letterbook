using System.Reflection;
using Letterbook.Core.Models;
using Letterbook.Generators;
using Microsoft.EntityFrameworkCore;

namespace Letterbook.Adapter.Db;

/// <summary>
/// This is the DbContext for most of the application. The actual data records (like the contents of profiles and
/// posts) will be managed here.
///
/// Feeds/timelines and notifications are managed separately, using a Timescale database.
/// There may be a need for search and/or graph databases in the future, and those would also be separate from this.
/// </summary>
public class RelationalContext : DbContext
{
	public DbSet<Note> Notes { get; set; }
	public DbSet<Profile> Profiles { get; set; }
	public DbSet<Account> Accounts { get; set; }
	public DbSet<Post> Posts { get; set; }
	public DbSet<ThreadContext> Threads { get; set; }
	public DbSet<Audience> Audience { get; set; }
	public DbSet<Peer> Peers { get; set; }
	public DbSet<ModerationReport> ModerationReports { get; set; }

	public RelationalContext(DbContextOptions<RelationalContext> context) : base(context)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		modelBuilder.Entity<ProfileClaims>().Navigation(claims => claims.Profile).AutoInclude();
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Properties<Uri>().HaveConversion<UriIdConverter, UriIdComparer>();
		AddTypedIdConversions(configurationBuilder);
	}

	private static void AddTypedIdConversions(ModelConfigurationBuilder configurationBuilder)
	{
		var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
		var converterTypes = allTypes.Where(type => type.GetCustomAttributes().Any(t => t is TypedIdEfConverterAttribute));
		foreach (var converterType in converterTypes)
		{
			foreach (var attribute in converterType.GetCustomAttributes())
			{
				if (attribute is TypedIdEfConverterAttribute converterAttribute)
				{
					configurationBuilder.Properties(converterAttribute.IdType).HaveConversion(converterType);//, converterAttribute.ComparerType);
				}
			}
		}
	}
}