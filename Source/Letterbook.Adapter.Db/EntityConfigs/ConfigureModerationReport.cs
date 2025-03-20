using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Letterbook.Adapter.Db.EntityConfigs;

public class ConfigureModerationReport : IEntityTypeConfiguration<Models.ModerationReport>
{
	public void Configure(EntityTypeBuilder<Models.ModerationReport> builder)
	{
		builder.HasMany(report => report.Moderators)
			.WithMany(account => account.ModeratedReports)
			.UsingEntity("ReportModerator");

		builder.HasMany(report => report.Subjects)
			.WithMany(profile => profile.ReportSubject)
			.UsingEntity("ReportSubjectProfile");

		builder.HasMany(report => report.Policies)
			.WithMany(policy => policy.RelatedReports)
			.UsingEntity("ReportByPolicy");

		builder.HasMany(report => report.RelatedPosts)
			.WithMany(post => post.RelatedReports)
			.UsingEntity("ReportSubjectPost");

		builder.HasOne(report => report.Reporter)
			.WithMany(profile => profile.Reports);

		// builder.Property(report => report.Reporter)
		// .HasColumnName("ReportAuthor");
	}
}