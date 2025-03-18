using Medo;

namespace Letterbook.Core.Models.Dto;

public class ThreadDto
{
	public Uuid7? Id { get; set; } = Uuid7.NewUuid7();
	public required Uri FediId { get; set; }
	public IEnumerable<PostDto> Posts { get; set; } = new List<PostDto>();
}