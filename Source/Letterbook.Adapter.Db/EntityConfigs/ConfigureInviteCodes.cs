using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureInviteCodes  : IEntityTypeConfiguration<Models.InviteCode>
{
	public void Configure(EntityTypeBuilder<Models.InviteCode> builder)
	{
		builder.Property(c => c.Code).HasMaxLength(14);
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id).ValueGeneratedNever();
		builder.HasIndex(c => c.Code);
		builder.HasOne(c => c.CreatedBy);
		builder.HasMany(c => c.RedeemedBy)
			.WithOne(a => a.InvitedBy);
	}
}