using Letterbook.Adapter.Db.NavigationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ConfigureFollowerRelations : IEntityTypeConfiguration<FollowerRelation>
{
    public void Configure(EntityTypeBuilder<FollowerRelation> builder)
    {
        builder.HasKey(relation => relation.Id);
        builder.HasIndex(relation => relation.Date);
    }
}