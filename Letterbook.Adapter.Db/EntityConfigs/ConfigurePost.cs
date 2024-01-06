using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigurePost : IEntityTypeConfiguration<Models.Post>
{
    public void Configure(EntityTypeBuilder<Models.Post> builder)
    {
        builder.HasIndex(post => post.IdUri);
        builder.HasIndex(post => post.Thread);
        builder.HasIndex(post => post.ContentRootId);
        
        builder.HasMany<Models.Mention>(post => post.AddressedTo)
            .WithOne("MentionedIn");
        builder.HasMany<Models.Profile>()
            .WithMany("CreatedPosts")
            .UsingEntity("PostsCreatedByProfile");
        builder.HasMany<Models.Profile>(post => post.LikesCollection)
            .WithMany("LikedPosts")
            .UsingEntity("PostsLikedByProfile");
        builder.HasMany<Models.Profile>(post => post.SharesCollection)
            .WithMany("SharedPosts")
            .UsingEntity("PostsSharedByProfile");
        builder.HasMany<Models.Audience>(post => post.Audience)
            .WithMany("Audience")
            .UsingEntity("PostsToAudience");
        builder.HasMany<Models.Post>(post => post.RepliesCollection)
            .WithOne(post => post.InReplyTo);
    }
}