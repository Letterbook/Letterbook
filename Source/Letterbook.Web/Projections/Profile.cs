using AutoMapper;
using Letterbook.Core.Models;

namespace Letterbook.Web.Projections;

public class Profile
{
	public static readonly DateTimeOffset PostsBefore = DateTimeOffset.MaxValue;
	public static readonly MapperConfiguration FromCoreModel =
		new(cfg => cfg.CreateProjection<Models.Profile, Profile>().ForMember(d => d.Posts,
			opt => opt.MapFrom(s => s.Posts.OrderByDescending(p => p.CreatedDate).Where(p => p.CreatedDate < PostsBefore).Take(100))));

	public ProfileId Id { get; set; }
	public string Handle { get; set; } = "";
	public string Authority { get; set; } = "";
	public string DisplayName { get; set; } = "";
	public string Description { get; set; } = "";
	public CustomField[] CustomFields { get; set; } = [];
	public DateTimeOffset Created { get; set; }
	public DateTimeOffset Updated { get; set; }
	public ActivityActorType Type { get; set; }
	public ICollection<Audience> Headlining { get; set; } = new HashSet<Audience>();
	public IList<Post> Posts { get; set; } = new List<Post>();
	public int FollowersEstimate { get; set; }
	public int FollowingEstimate { get; set; }
	public int PostsEstimate { get; set; }
	public int FollowersCount { get; set; }
	public int FollowingCount { get; set; }
	public int PostsCount { get; set; }
}