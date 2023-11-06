using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureSigningKey : IEntityTypeConfiguration<Models.SigningKey>
{
    public void Configure(EntityTypeBuilder<Models.SigningKey> builder)
    {
        builder.Property(key => key.PrivateKey).HasConversion(
            memory => (memory != null) ? memory.Value.ToArray() : null, // To Db Type
            bytes => new ReadOnlyMemory<byte>(bytes) // To CLR Type
        );
        builder.Property(key => key.PublicKey).HasConversion(
            memory => memory.ToArray(),
            bytes => new ReadOnlyMemory<byte>(bytes)
        );
    }
}