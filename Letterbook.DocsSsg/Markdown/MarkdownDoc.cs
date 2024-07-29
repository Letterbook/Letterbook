namespace Letterbook.DocsSsg.Markdown;

public class MarkdownDoc
{
	public required string Title { get; set; }
	public required string Path { get; set; }
	public required string FileName { get; set; }
	public required string Slug { get; set; }
	public required string Source { get; set; }
	public required string HtmlLede { get; set; }
	public required string Html { get; set; }
	public required DateTime Date { get; set; }
	public List<string> Tags { get; set; } = new();
	public List<string> Authors { get; set; } = new();
	public bool Draft { get; set; } = false;
}

public class MarkdownAuthor : MarkdownDoc
{
	public required string Id { get; set; }
	public required string DisplayName { get; set; }
	public required string Url { get; set; }
	public required List<KeyValuePair<string, string>> Socials { get; set; }
}