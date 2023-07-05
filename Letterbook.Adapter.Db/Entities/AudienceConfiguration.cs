using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class AudienceConfiguration : IEntityTypeConfiguration<Audience>
{
    public void Configure(EntityTypeBuilder<Audience> builder)
    {
        builder.HasMany(entity => entity.Members)
            .WithMany(entity => entity.Audiences);
    }
}
