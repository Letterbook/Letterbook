using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureThreadContext : IEntityTypeConfiguration<Models.ThreadContext>
{
	public void Configure(EntityTypeBuilder<Models.ThreadContext> builder)
	{
		builder.HasIndex(conversation => conversation.FediId);
		builder.Property(thread => thread.RootId).IsRequired();
		builder.HasIndex(thread => thread.RootId);

		builder.HasMany<Models.Post>(conversation => conversation.Posts)
			.WithOne(post => post.Thread);

		builder.Ignore(thread => thread.Heuristics);
	}
}