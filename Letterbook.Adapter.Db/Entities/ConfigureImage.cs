using System.Net.Mime;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ConfigureImage : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasMany<Models.Mention>(entity => entity.Mentions);
        builder.Property(entity => entity.MimeType).HasConversion<string>(v => v.ToString(), v => new ContentType(v));
    }
}