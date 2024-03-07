namespace Letterbook.Api.Dto;

public class ContentDto
{
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public Uri? Source { get; set; }
    public int? SortKey { get; set; } = 0;
    public required string Type { get; set; }

    // Required for Notes
    public string? Text { get; set; }
}