using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureLinkedProfile : IEntityTypeConfiguration<Models.ProfileClaims>
{
	public void Configure(EntityTypeBuilder<Models.ProfileClaims> builder)
	{
		builder.HasOne<Models.Profile>(access => access.Profile)
			.WithMany(profile => profile.Accessors);
		builder.HasOne<Models.Account>(access => access.Account)
			.WithMany(account => account.LinkedProfiles);
		builder.Property(link => link.Claims).HasColumnType("jsonb");
	}
}