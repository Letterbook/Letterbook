using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureProfileAccess : IEntityTypeConfiguration<Models.ProfileAccess>
{
	public void Configure(EntityTypeBuilder<Models.ProfileAccess> builder)
	{
		builder.Property(pa => pa.Id)
			.ValueGeneratedNever();
	}
}