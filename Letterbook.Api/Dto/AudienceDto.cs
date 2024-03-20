using Medo;

namespace Letterbook.Api.Dto;

public class AudienceDto
{
	/// <example>0672016s27hx3fjxmn5ic1hzq</example>
    public Uuid7? Id { get; set; } = Uuid7.NewUuid7();
    public Uri? FediId { get; set; }
    public string? SourceId { get; set; }
    public string? SourceType { get; set; }
}