using Medo;

namespace Letterbook.Api.Dto;

public class ThreadDto
{
	public Uuid7? Id { get; set; } = Uuid7.NewUuid7();
	public required Uri FediId { get; set; }
}