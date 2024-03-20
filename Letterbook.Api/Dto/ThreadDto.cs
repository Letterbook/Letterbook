using Medo;

namespace Letterbook.Api.Dto;

public class ThreadDto
{
	/// <example>0672016s27hx3fjxmn5ic1hzq</example>
	public Uuid7? Id { get; set; } = Uuid7.NewUuid7();
	public required Uri FediId { get; set; }
}