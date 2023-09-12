using System.Net.Mime;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureImage : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.OwnsMany<Models.Mention>(entity => entity.Mentions)
            .WithOwner();
        builder.HasMany<Profile>(image => image.Creators)
            .WithMany("CreatedImages")
            .UsingEntity("ImagesCreatedByProfile");
        builder.Property(entity => entity.MimeType).HasConversion<string>(v => v.ToString(), v => new ContentType(v));
    }
}