namespace Letterbook.Core.Models.Dto;

/// <summary>
/// A view of ModerationReports that can be shared with—or submitted by—a regular member
/// </summary>
public class MemberModerationReportDto
{
	public ModerationReportId Id { get; set; }
	public required string Summary { get; set; }
	public ICollection<ModerationPolicyId> Policies { get; set; } = new HashSet<ModerationPolicyId>();
	public ProfileId Reporter { get; set; }
	public ICollection<ProfileId> Subjects { get; set; } = new HashSet<ProfileId>();
	public ICollection<PostId> RelatedPosts { get; set; } = new HashSet<PostId>();
	public IEnumerable<Uri> Forwarded { get; set; } = new List<Uri>();
}

public class FullModerationReportDto : MemberModerationReportDto
{
	public Uri? FediId { get; set; }
	public required ThreadId Context { get; set; }
	public ICollection<Guid> Moderators { get; set; } = new HashSet<Guid>();
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Closed { get; set; } = DateTimeOffset.UtcNow;
	public IEnumerable<ModerationRemarkDto> Remarks { get; set; } = new List<ModerationRemarkDto>();
}

public class ModerationRemarkDto
{
	public ModerationRemarkId Id { get; set; }
	public required ModerationReportId Report { get; set; }
	public required Guid Author { get; set; }
	public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
	public DateTimeOffset Updated { get; set; } = DateTimeOffset.Now;
	public required string Text { get; set; }
}