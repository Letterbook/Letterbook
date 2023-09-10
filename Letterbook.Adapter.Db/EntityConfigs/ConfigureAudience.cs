using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureAudience : IEntityTypeConfiguration<Models.Audience>
{
    public void Configure(EntityTypeBuilder<Models.Audience> builder)
    {
        builder.Ignore(e => e.LocalId);
        builder.HasMany<Models.Profile>()
            .WithMany()
            .UsingEntity("AudienceProfileMembers");
    }
}