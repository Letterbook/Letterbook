namespace Letterbook.Core.Models.Dto;

public class ModerationPolicyDto
{
	public ModerationPolicyId Id { get; set; }
	public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset Retired { get; set; } = DateTimeOffset.MaxValue;
	public string Title { get; set; } = "";
	public string Summary { get; set; } = "";
	public string Policy { get; set; } = "";
}