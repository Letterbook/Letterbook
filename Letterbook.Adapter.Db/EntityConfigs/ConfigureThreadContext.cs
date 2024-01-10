using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureThreadContext : IEntityTypeConfiguration<Models.ThreadContext>
{
    public void Configure(EntityTypeBuilder<Models.ThreadContext> builder)
    {
        builder.HasIndex(conversation => conversation.FediId);

        builder.HasOne<Models.Post>(conversation => conversation.Root);
        builder.HasMany<Models.Post>(conversation => conversation.Posts)
            .WithOne(post => post.Thread);

        builder.Ignore(thread => thread.Heuristics);
    }
}