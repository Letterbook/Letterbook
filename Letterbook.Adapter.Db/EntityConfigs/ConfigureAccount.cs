using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureAccount : IEntityTypeConfiguration<Models.Account>
{
    public void Configure(EntityTypeBuilder<Models.Account> builder)
    {
        builder.Property(account => account.Id)
            .ValueGeneratedNever();
    }
}
