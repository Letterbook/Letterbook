using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.TimescaleFeeds.Entities;

public class ConfigureTimelinePost : IEntityTypeConfiguration<TimelinePost>
{
	private static readonly HashSet<string> ProfileProps =
	[
		nameof(Profile.Id), nameof(Profile.FediId), nameof(Profile.DisplayName), nameof(Profile.Authority)
	];

	public void Configure(EntityTypeBuilder<TimelinePost> builder)
	{
		builder.HasNoKey();
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