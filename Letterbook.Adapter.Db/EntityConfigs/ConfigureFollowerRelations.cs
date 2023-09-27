using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureFollowerRelations : IEntityTypeConfiguration<Models.FollowerRelation>
{
    public void Configure(EntityTypeBuilder<Models.FollowerRelation> builder)
    {
        builder.HasKey(relation => relation.Id);
        builder.HasIndex(relation => relation.Date);
        builder.HasOne<Models.Profile>(relation => relation.Subject)
            .WithMany(profile => profile.FollowersCollection);
        builder.HasOne<Models.Profile>(relation => relation.Follows)
            .WithMany(profile => profile.Following);
    }
}