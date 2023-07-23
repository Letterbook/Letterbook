using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ConfigureNote : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasMany<Models.Mention>(entity => entity.Mentions);
        builder.HasMany<Profile>(entity => entity.Creators)
            .WithMany("CreatedNotes");
        builder.HasMany<Profile>(entity => entity.LikedBy)
            .WithMany("LikedNotes");
        builder.HasMany<Profile>(entity => entity.BoostedBy)
            .WithMany("BoostedNotes");
    }
}