using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureLinkedProfile : IEntityTypeConfiguration<Models.ProfileAccess>
{
    public void Configure(EntityTypeBuilder<Models.ProfileAccess> builder)
    {
        // builder.HasKey(link => new { link.Account, link.Profile });
        // builder.Property(link => link.Id)
            // .ValueGeneratedNever();
        // builder.Property(link => link)
        builder.HasOne<Models.Profile>(access => access.Profile)
            .WithMany(profile => profile.Accessors);
        builder.HasOne<Models.Account>(access => access.Account)
            .WithMany(account => account.LinkedProfiles);
        // builder.HasOne<Models.Account>(link => link.Account).WithMany();
        // builder.HasOne<Models.Profile>(link => link.Profile).WithMany();
    }
}