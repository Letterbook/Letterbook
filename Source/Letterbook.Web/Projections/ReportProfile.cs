using AutoMapper;

namespace Letterbook.Web.Projections;

public class ReportProfile
{
	public static MapperConfiguration FromCoreModel(Models.ModerationReportId reportId) =>
		new(cfg => cfg.CreateProjection<Models.Profile, ReportProfile>()
			.ForMember(d => d.ReportId, opt => opt.MapFrom(s => reportId))
			.ForMember(d => d.Reports, opt => opt.MapFrom(s => s.Reports.OrderByDescending(r => r.Created).Take(20)))
			.ForMember(d => d.ReportSubject, opt => opt.MapFrom(s => s.ReportSubject.OrderByDescending(r => r.Created).Take(20)))
		);

	public Models.ModerationReportId ReportId { get; set; }
	public Models.ProfileId Id { get; set; }
	public string Handle { get; set; } = "";
	public string Authority { get; set; } = "";
	public string DisplayName { get; set; } = "";
	public DateTimeOffset Created { get; set; }
	public DateTimeOffset Updated { get; set; }
	public Models.ActivityActorType Type { get; set; }
	public int FollowersEstimate { get; set; }
	public int FollowingEstimate { get; set; }
	public ICollection<Models.ModerationReport> ReportSubject = new HashSet<Models.ModerationReport>();
	public ICollection<Models.ModerationReport> Reports = new HashSet<Models.ModerationReport>();
	public IDictionary<Models.Restrictions, DateTimeOffset> Restrictions { get; set; } = new Dictionary<Models.Restrictions, DateTimeOffset>();
	public int PostsEstimate { get; set; }
	public int FollowersCount { get; set; }
	public int FollowingCount { get; set; }
	public int PostsCount { get; set; }
}