using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureProfileAccess : IEntityTypeConfiguration<Models.ProfileClaims>
{
	public void Configure(EntityTypeBuilder<Models.ProfileClaims> builder)
	{
		builder.Property(pa => pa.Id)
			.ValueGeneratedNever();
	}
}