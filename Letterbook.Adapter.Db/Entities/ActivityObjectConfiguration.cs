using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.Entities;

public class ActivityObjectConfiguration : IEntityTypeConfiguration<ApObject>
{
    public void Configure(EntityTypeBuilder<ApObject> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.HasOne<Actor>(entity => entity.Actor);
        builder.HasMany<Actor>(entity => entity.AddressedTo)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.To));
        builder.HasMany<Actor>(entity => entity.AddressedBto)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.Bto));

        builder.HasMany<Actor>(entity => entity.AddressedCc)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.Cc));

        builder.HasMany<Actor>(entity => entity.AddressedBcc)
            .WithMany()
            .UsingEntity<JoinObjectActor>(b => b.HasDiscriminator(j => j.Relationship).HasValue(AddressedRelationship.Bcc));

        builder.HasMany<Audience>(entity => entity.Audience)
            .WithMany(e => e.Objects)
            .UsingEntity("JoinObjectAudience");
    }
}