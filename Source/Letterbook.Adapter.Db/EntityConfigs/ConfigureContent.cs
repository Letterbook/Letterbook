using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureContent : IEntityTypeConfiguration<Models.Content>
{
	public void Configure(EntityTypeBuilder<Models.Content> builder)
	{
		builder.HasDiscriminator();
		builder.HasKey(note => note.Id);
		builder.HasIndex(note => note.FediId);
		builder.Property(note => note.ContentType).HasConversion<ContentTypeConverter>();
		builder.Property(note => note.Id).ValueGeneratedNever();
	}
}