using System.Reflection;
using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Letterbook.Core.Models;
using Letterbook.Generators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Letterbook.Adapter.TimescaleFeeds;

public class FeedsContext : DbContext
{
	public DbSet<TimelinePost> Timelines { get; set; } = null!;

	// Called by the designer to create and run migrations
	public FeedsContext(DbContextOptions<FeedsContext> options) : base(options)
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

	internal class UriIdConverter : ValueConverter<Uri, string>
	{
		public UriIdConverter() : base(uri => uri.ToString(), s => new Uri(s))
		{
		}
	}

	internal class UriIdComparer : ValueComparer<Uri>
	{
		public UriIdComparer() : base((u1, u2) => (u1 != null && u2 != null) && u1.ToString() == u2.ToString(),
			uri => uri.ToString().GetHashCode())
		{
		}
	}
}