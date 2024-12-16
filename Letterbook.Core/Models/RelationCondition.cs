using Letterbook.Core.Values;

namespace Letterbook.Core.Models;

public class RelationCondition
{
	public DateTimeOffset Beginning { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Expiration { get; set; } = DateTimeOffset.MaxValue;
	public ConditionKind Condition { get; set; }
}