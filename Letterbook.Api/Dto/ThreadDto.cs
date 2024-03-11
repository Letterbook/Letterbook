namespace Letterbook.Api.Dto;

public class ThreadDto
{
	/// <example>0672016s27hx3fjxmn5ic1hzq</example>
	public string? Id { get; set; }
	public required Uri FediId { get; set; }
}