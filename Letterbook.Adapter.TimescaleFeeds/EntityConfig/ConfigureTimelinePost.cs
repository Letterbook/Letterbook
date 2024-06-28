using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.TimescaleFeeds.EntityConfig;

public class ConfigureTimelinePost : IEntityTypeConfiguration<TimelinePost>
{
	public void Configure(EntityTypeBuilder<TimelinePost> builder)
	{
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

		builder.Property(model => model.SharedBy).HasColumnType("jsonb");
	}
}