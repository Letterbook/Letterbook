using Letterbook.Generators;
using Medo;

namespace Letterbook.Core.Models;

public partial record struct ModerationReportId(Uuid7 Id) : ITypedId<Uuid7>;

public class ModerationReport
{
	public ModerationReportId Id { get; set; }
	public Uri? FediId => Context.FediId;
	public required string Summary { get; set; }
	public required ThreadContext Context { get; set; }
	public ICollection<Account> Moderators { get; set; } = new HashSet<Account>();
	public ICollection<ModerationPolicy> Policies { get; set; } = new HashSet<ModerationPolicy>();
	public Profile? Reporter { get; set; }
	public ICollection<Profile> Subjects { get; set; } = new HashSet<Profile>();
	public ICollection<Post> RelatedPosts { get; set; } = new HashSet<Post>();
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Closed { get; set; } = DateTimeOffset.UtcNow;
	public IEnumerable<ModerationRemark> Remarks { get; set; } = new List<ModerationRemark>();
}