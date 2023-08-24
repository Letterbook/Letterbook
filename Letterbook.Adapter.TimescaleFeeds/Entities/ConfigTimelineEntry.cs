using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.TimescaleFeeds.Entities;

public class ConfigTimelineEntry : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.HasNoKey();
        builder.HasIndex(model => model.Time);
        builder.HasIndex(model => model.AudienceKey);
        builder.HasIndex(model => model.EntityId);
        builder.HasIndex(model => model.CreatedBy).HasMethod("GIN");
    }
}
