using Medo;

namespace Letterbook.Api.Dto;

public class ContentDto
{
	/// <example>0672016s27hx3fjxmn5ic1hzq</example>
	public Uuid7? Id { get; set; } = Uuid7.NewUuid7();
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public Uri? Source { get; set; }
    public int? SortKey { get; set; } = 0;
    public required string Type { get; set; }

    /// <summary>Required for Notes. The text of the Note.</summary>
    public string? Text { get; set; }
}