using Letterbook.Generators;
using Medo;

namespace Letterbook.Core.Models;

[GenerateTypedId]
public partial record struct ModerationRemarkId(Uuid7 Id);

public class ModerationRemark : IComparable<ModerationRemark>
{
	public ModerationRemarkId Id { get; set; } = Uuid7.NewUuid7();
	public required ModerationReport Report { get; set; }
	public required Account Author { get; set; }
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Updated { get; set; } = DateTimeOffset.UtcNow;
	public required string Text { get; set; }

	public int CompareTo(ModerationRemark? other)
	{
		if (ReferenceEquals(this, other)) return 0;
		if (other is null) return 1;
		return Created.CompareTo(other.Created);
	}
}