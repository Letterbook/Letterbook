using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureAccount : IEntityTypeConfiguration<Models.Account>
{
    public void Configure(EntityTypeBuilder<Models.Account> builder)
    {
        builder.HasIndex(account => account.Email);
        builder.HasIndex(account => account.UserName);
    }
}
