using Medo;

namespace Letterbook.Api.Dto;

public class AudienceDto
{
	public Uri? FediId { get; set; }
	public string? SourceId { get; set; }
	public string? SourceType { get; set; }
}