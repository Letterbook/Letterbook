using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.TimescaleFeeds.EntityConfig;

public class ConfigureTimelinePost : IEntityTypeConfiguration<TimelinePost>
{
	public void Configure(EntityTypeBuilder<TimelinePost> builder)
	{
		// This key is purely for EFCore's benefit, we'll likely never (need to) query on it
		// TODO(profiling): Evaluate the performance impact of this key, and compare with using `ExecuteSqlAsync` to do custom INSERTs
		// If ExecuteInsert() ever happens, use that instead and drop this index
		// https://github.com/dotnet/efcore/issues/29897
		// Also, explore using BulkExtensions https://www.nuget.org/packages/EFCore.BulkExtensions
		builder.HasKey(model => new { model.Time, model.PostId, model.AudienceId });

		builder.HasIndex(model => model.Time);
		// Hash indexes do what it says on the tin
		// They're faster than BTrees for equality but support no other comparisons
		builder.HasIndex(model => model.AudienceId).HasMethod("Hash");
		builder.HasIndex(model => model.PostId).HasMethod("Hash");
		builder.HasIndex(model => model.Authority).HasMethod("Hash");

		// GIN is an index optimized for multi-value columns, like json arrays
		// It supports comparisons for equality (=, is, in) and inequality (=>, >, <, <=)
		builder.HasIndex(model => model.Creators).HasMethod("GIN");
		builder.Property(model => model.Creators).HasColumnType("jsonb");
	}
}