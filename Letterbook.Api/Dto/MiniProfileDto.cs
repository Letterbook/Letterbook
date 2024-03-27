namespace Letterbook.Api.Dto;

public class MiniProfileDto
{
	/// <example>0672016s27hx3fjxmn5ic1hzq</example>
	public string? Id { get; set; }
	public Uri? FediId { get; set; }
	public string? DisplayName { get; set; }
	public string? Handle { get; set; }
}