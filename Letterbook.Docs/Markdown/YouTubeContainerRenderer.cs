using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Letterbook.Docs.Markdown;

public class YouTubeContainerRenderer : HtmlObjectRenderer<CustomContainer>
{
	public string? ContainerClass { get; set; } // = "flex justify-center";
	public string? Class { get; set; } = "w-full mx-4 my-4";

	protected override void Write(HtmlRenderer renderer, CustomContainer obj)
	{
		renderer.EnsureLine();

		var videoId = (obj.Arguments ?? "").TrimEnd(':');
		if (string.IsNullOrEmpty(videoId))
		{
			renderer.WriteLine("<!-- youtube: Missing YouTube Video Id -->");
			return;
		}

		var title = ((obj.Count > 0 ? obj[0] as ParagraphBlock : null)?.Inline?.FirstChild as LiteralInline)?.Content
			.ToString() ?? "";
		if (ContainerClass != null) renderer.WriteLine($"<div class=\"{ContainerClass}\">");
		renderer.WriteLine(
			$"<lite-youtube class=\"{Class}\" width=\"560\" height=\"315\" videoid=\"{videoId}\" playlabel=\"{title}\" style=\"background-image:url('https://img.youtube.com/vi/{videoId}/maxresdefault.jpg')\"></lite-youtube>");
		if (ContainerClass != null) renderer.WriteLine("</div>");
	}
}