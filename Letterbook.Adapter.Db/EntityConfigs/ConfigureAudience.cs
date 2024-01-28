using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureAudience : IEntityTypeConfiguration<Models.Audience>
{
    public void Configure(EntityTypeBuilder<Models.Audience> builder)
    {
        builder.HasMany<Models.Profile>(audience => audience.Members)
            .WithMany(profile => profile.Audiences)
            .UsingEntity("AudienceProfileMembers");
        builder.HasOne<Models.Profile>(audience => audience.Source);
        builder.HasIndex(audience => audience.FediId);
    }
}