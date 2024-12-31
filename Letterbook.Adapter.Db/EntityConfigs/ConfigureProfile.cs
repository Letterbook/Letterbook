using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureProfile : IEntityTypeConfiguration<Models.Profile>
{
	public void Configure(EntityTypeBuilder<Models.Profile> builder)
	{
		builder.Property(profile => profile.Id)
			.ValueGeneratedNever();
		builder.HasIndex(profile => profile.FediId);
		builder.HasIndex(profile => profile.Authority);
		builder.HasOne<Models.Account>(profile => profile.OwnedBy);
		builder.Property(profile => profile.CustomFields).HasColumnType("jsonb");
		builder.HasMany(profile => profile.Keys);
		builder.HasMany<Models.FollowerRelation>(profile => profile.FollowingCollection)
			.WithOne(relation => relation.Follower)
			.IsRequired();
		builder.HasMany<Models.FollowerRelation>(profile => profile.FollowersCollection)
			.WithOne(relation => relation.Follows)
			.IsRequired();
		builder.Property(profile => profile.Restrictions).HasColumnType("jsonb");
	}
}