namespace Letterbook.Docs;

public static class MarkdownExtensions
{
	public static string? StripFrontmatter(string? content)
	{
		if (content == null)
			return null;
		var startPos = content.IndexOf("---", StringComparison.CurrentCulture);
		if (startPos == -1)
			return content;
		var endPos = content.IndexOf("---", startPos + 3, StringComparison.Ordinal);
		if (endPos == -1)
			return content;
		return content.Substring(endPos + 3).Trim();
	}
}