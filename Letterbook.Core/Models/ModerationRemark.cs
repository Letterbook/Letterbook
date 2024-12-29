namespace Letterbook.Core.Models;

public class ModerationRemark : IComparable<ModerationRemark>
{
	public required ModerationReport Report { get; set; }
	public required Account Author { get; set; }
	public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
	public DateTimeOffset Updated { get; set; } = DateTimeOffset.Now;
	public required string Text { get; set; }

	public int CompareTo(ModerationRemark? other)
	{
		if (ReferenceEquals(this, other)) return 0;
		if (other is null) return 1;
		return Created.CompareTo(other.Created);
	}
}