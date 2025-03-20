using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureModerationPolicy : IEntityTypeConfiguration<Models.ModerationPolicy>
{
	public void Configure(EntityTypeBuilder<Models.ModerationPolicy> builder)
	{
		builder.Property(policy => policy.Id).ValueGeneratedOnAdd();
	}
}