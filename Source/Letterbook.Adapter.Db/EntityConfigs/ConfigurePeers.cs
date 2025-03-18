using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigurePeers : IEntityTypeConfiguration<Models.Peer>
{
	public void Configure(EntityTypeBuilder<Models.Peer> builder)
	{
		builder.HasKey(peer => peer.Authority);
		builder.Property(peer => peer.Restrictions).HasColumnType("jsonb");
	}
}