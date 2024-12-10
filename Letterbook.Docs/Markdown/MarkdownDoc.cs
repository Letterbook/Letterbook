using System.Diagnostics.CodeAnalysis;

namespace Letterbook.Docs.Markdown;

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
	public string? Canonical { get; set; }
	public MarkdownImage? Image { get; set; }
	public int Order { get; set; }
	public List<string> Tags { get; set; } = new();
	public List<string> Authors { get; set; } = new();
	public bool Draft { get; set; } = false;

	public MarkdownDoc()
	{
	}

	[SetsRequiredMembers]
	public MarkdownDoc(MarkdownDoc from)
	{
		Title = from.Title;
		Path = from.Path;
		FileName = from.FileName;
		Slug = from.Slug;
		Source = from.Source;
		HtmlLede = from.HtmlLede;
		Html = from.Html;
		Date = from.Date;
		Order = from.Order;
		Tags = from.Tags;
		Authors = from.Authors;
		Draft = from.Draft;
	}
}

public class AdrStatus
{
	public required string Status { get; set; }
	public required DateTimeOffset Date { get; set; }
}

public class MarkdownAdr : MarkdownDoc
{
	public string? DiscussionUrl { get; set; }
	public AdrStatus[] StatusHistory { get; set; } = [];
	public required string Code { get; set; }
}

public class LabelledValue
{
	public required string Label { get; set; }
	public required string Value { get; set; }
}

public class MarkdownAuthor : MarkdownDoc
{
	public required string Id { get; set; }
	public required string DisplayName { get; set; }
	public string? Url { get; set; }
	public List<LabelledValue> Socials { get; set; } = [];
}

public class MarkdownImage
{
	public required string Href { get; set; }
	public required string Alt { get; set; }
	public string? Attribution { get; set; }
}

public class MarkdownCategory : MarkdownDoc
{
	public string? Category { get; set; }

	public MarkdownCategory()
	{
	}

	[SetsRequiredMembers]
	public MarkdownCategory(MarkdownDoc doc) : base(doc)
	{
	}
}