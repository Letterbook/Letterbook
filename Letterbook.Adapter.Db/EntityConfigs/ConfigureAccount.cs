using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureAccount : IEntityTypeConfiguration<Models.Account>
{
    public void Configure(EntityTypeBuilder<Models.Account> builder)
    {
        // const string identityKey = nameof(Models.Account.Identity) + "Id";
        // builder.Property<Guid>(identityKey);
        // builder.HasIndex(identityKey);
        // builder.HasOne<Models.AccountIdentity>(account => account.Identity)
            // .WithOne(identity => identity.Account)
            // .HasForeignKey<Models.AccountIdentity>("AccountId");
    }
}
