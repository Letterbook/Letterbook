using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Letterbook.Adapter.Db.Entities;

public class ConfigureLinkedProfile : IEntityTypeConfiguration<Models.LinkedProfile>
{
    public void Configure(EntityTypeBuilder<Models.LinkedProfile> builder)
    {
        builder.HasKey("AccountId", "ProfileId");
        builder.HasOne<Models.Account>(link => link.Account);
        builder.HasOne<Models.Profile>(link => link.Profile);
    }
}