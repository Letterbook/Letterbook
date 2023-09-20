using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureProfile : IEntityTypeConfiguration<Models.Profile>
{
    public void Configure(EntityTypeBuilder<Models.Profile> builder)
    {
        builder.HasKey(profile => profile.Id);
        builder.HasIndex(profile => profile.LocalId);
        builder.HasOne<Models.Account>(profile => profile.OwnedBy);
        builder.Property(profile => profile.CustomFields).HasColumnType("jsonb");
    }
}