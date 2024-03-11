namespace Letterbook.Api.Dto;

public class ContentDto
{
	/// <example>0672016s27hx3fjxmn5ic1hzq</example>
	public string? Id { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public Uri? Source { get; set; }
    public int? SortKey { get; set; } = 0;
    public required string Type { get; set; }

    // Required for Notes
    public string? Text { get; set; }
}