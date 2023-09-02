using Letterbook.Adapter.Db.NavigationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ConfigureProfile : IEntityTypeConfiguration<Models.Profile>
{
    public void Configure(EntityTypeBuilder<Models.Profile> builder)
    {
        builder.HasKey(profile => profile.Id);
        builder.HasIndex(profile => profile.LocalId);
        builder.HasOne<Models.Account>(profile => profile.OwnedBy);
        builder.HasMany<Models.Profile>(profile => profile.FollowersCollection)
            .WithMany("Following")
            .UsingEntity<FollowerRelation>();
    }
}