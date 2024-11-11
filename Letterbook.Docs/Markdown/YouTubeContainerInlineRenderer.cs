using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using ServiceStack.Text;

namespace Letterbook.Docs.Markdown;

public class YouTubeContainerInlineRenderer : HtmlObjectRenderer<CustomContainerInline>
{
	public string? ContainerClass { get; set; } = "flex justify-center";
	public string? Class { get; set; } = "w-full mx-4 my-4";

	protected override void Write(HtmlRenderer renderer, CustomContainerInline obj)
	{
		var videoId = obj.FirstChild is LiteralInline literalInline
			? literalInline.Content.AsSpan().RightPart(' ').ToString()
			: null;
		if (string.IsNullOrEmpty(videoId))
			return;

		if (ContainerClass != null) renderer.WriteLine($"<div class=\"{ContainerClass}\">");
		renderer.WriteLine(
			$"<lite-youtube class=\"{Class}\" width=\"560\" height=\"315\" videoid=\"{videoId}\" style=\"background-image:url('https://img.youtube.com/vi/{videoId}/maxresdefault.jpg')\"></lite-youtube>");
		if (ContainerClass != null) renderer.WriteLine("</div>");
	}
}