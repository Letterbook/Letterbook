using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ConfigureNote : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.OwnsMany<Models.Mention>(entity => entity.Mentions)
            .WithOwner();
        builder.HasMany<Profile>(entity => entity.Creators)
            .WithMany("CreatedNotes")
            .UsingEntity("NotesCreatedByProfile");
        builder.HasMany<Profile>(entity => entity.LikedBy)
            .WithMany("LikedNotes")
            .UsingEntity("NotesLikedByProfile");
        builder.HasMany<Profile>(entity => entity.BoostedBy)
            .WithMany("BoostedNotes")
            .UsingEntity("NotesBoostedByProfile");
    }
}