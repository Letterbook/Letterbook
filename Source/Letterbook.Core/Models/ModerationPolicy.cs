using Letterbook.Generators;

namespace Letterbook.Core.Models;

[GenerateTypedId]
public partial record struct ModerationPolicyId(int Id);

public class ModerationPolicy
{
	public ModerationPolicyId Id { get; set; }
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Retired { get; set; } = DateTimeOffset.MaxValue;
	public string Title { get; set; } = "";
	public string Summary { get; set; } = "";
	public string Policy { get; set; } = "";
	public ICollection<ModerationReport> RelatedReports { get; set; } = new HashSet<ModerationReport>();

	public bool Retire()
	{
		if (Retired <= DateTimeOffset.UtcNow)
			return false;
		Retired = DateTimeOffset.UtcNow;
		return true;
	}
}