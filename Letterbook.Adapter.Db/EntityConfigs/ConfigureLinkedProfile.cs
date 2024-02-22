using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureLinkedProfile : IEntityTypeConfiguration<Models.LinkedProfile>
{
    public void Configure(EntityTypeBuilder<Models.LinkedProfile> builder)
    {
        builder.HasOne<Models.Account>(link => link.Account);
        builder.HasOne<Models.Profile>(link => link.Profile);
    }
}