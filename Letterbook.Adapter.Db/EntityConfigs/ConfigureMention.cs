using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureMention : IEntityTypeConfiguration<Models.Mention>
{
	public void Configure(EntityTypeBuilder<Models.Mention> builder)
	{
		builder.HasKey(mention => new { mention.SubjectId, mention.SourceId });
		builder
			.HasOne(m => m.Source)
			.WithMany(p => p.AddressedTo)
			.HasForeignKey(m => m.SourceId);
		builder.HasOne(m => m.Subject)
			.WithMany()
			.HasForeignKey(m => m.SubjectId);
	}
}
