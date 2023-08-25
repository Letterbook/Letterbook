using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.TimescaleFeeds.Entities;

public class ConfigTimelineEntry : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.HasNoKey();
        builder.HasIndex(model => model.Time);
        // Hash indexes do what it says on the tin
        // They're faster than BTrees for equality but support no other comparisons
        builder.HasIndex(model => model.AudienceKey).HasMethod("Hash");
        builder.HasIndex(model => model.EntityId).HasMethod("Hash");
        builder.HasIndex(model => model.Authority).HasMethod("Hash");
        // GIN is an index optimized for multi-value columns, like arrays
        // It supports comparisons for equality (=, is, in) and inequality (=>, >, <, <=) 
        builder.HasIndex(model => model.CreatedBy).HasMethod("GIN");
    }
}
