using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.TimescaleFeeds.Entities;

public class ConfigTimelineRecord : IEntityTypeConfiguration<TimelineRecord>
{
    public void Configure(EntityTypeBuilder<TimelineRecord> builder)
    {
        builder.HasNoKey();
        builder.HasIndex(model => model.Time);
        builder.HasIndex(model => model.AudienceKey);
        builder.HasIndex(model => model.EntityId);
        builder.HasIndex(model => model.CreatedBy).HasMethod("GIN");
    }
}
