using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ActivityObjectConfiguration : IEntityTypeConfiguration<ApObject>
{
    public void Configure(EntityTypeBuilder<ApObject> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.HasOne<Profile>(entity => entity.Profile);
        builder.HasMany<Profile>(entity => entity.AddressedTo)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.To));
        builder.HasMany<Profile>(entity => entity.AddressedBto)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.Bto));

        builder.HasMany<Profile>(entity => entity.AddressedCc)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.Cc));

        builder.HasMany<Profile>(entity => entity.AddressedBcc)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.Bcc));

        builder.HasMany<Audience>(entity => entity.Audience)
            .WithMany(e => e.Objects)
            .UsingEntity("JoinObjectAudience");
    }
}