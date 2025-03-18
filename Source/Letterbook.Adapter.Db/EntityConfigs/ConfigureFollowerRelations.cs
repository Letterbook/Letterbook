using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureFollowerRelations : IEntityTypeConfiguration<Models.FollowerRelation>
{
	public void Configure(EntityTypeBuilder<Models.FollowerRelation> builder)
	{
		builder.HasKey(relation => relation.Id);
		builder.Property(relation => relation.Id)
			.ValueGeneratedNever();
		builder.HasIndex(relation => relation.Date);
		// builder.OwnsMany(relation => relation.Conditions);
		builder.Property(relation => relation.Conditions).HasColumnType("jsonb");
	}
}