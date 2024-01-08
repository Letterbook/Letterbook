using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigurePost : IEntityTypeConfiguration<Models.Post>
{
    public void Configure(EntityTypeBuilder<Models.Post> builder)
    {
        builder.HasIndex(post => post.FediId);
        builder.HasIndex(post => post.ContentRootId);

        builder.HasMany<Models.Content>(post => post.Contents)
            .WithOne(content => content.Post);
        builder.OwnsMany<Models.Mention>(post => post.AddressedTo);
        builder.HasMany<Models.Profile>(post => post.Creators)
            .WithMany("CreatedPosts")
            .UsingEntity("PostsCreatedByProfile");
        builder.HasMany<Models.Profile>(post => post.LikesCollection)
            .WithMany("LikedPosts")
            .UsingEntity("PostsLikedByProfile");
        builder.HasMany<Models.Profile>(post => post.SharesCollection)
            .WithMany("SharedPosts")
            .UsingEntity("PostsSharedByProfile");
        builder.HasMany<Models.Audience>(post => post.Audience)
            .WithMany("Post")
            .UsingEntity("PostsToAudience");
        builder.HasMany<Models.Post>(post => post.RepliesCollection)
            .WithOne(post => post.InReplyTo);
    }
}