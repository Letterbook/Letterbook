using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ConfigureProfile : IEntityTypeConfiguration<Models.Profile>
{
    public void Configure(EntityTypeBuilder<Models.Profile> builder)
    {
        builder.HasOne<Models.Account>(profile => profile.OwnedBy);
    }
}