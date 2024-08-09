using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack.IO;
using ServiceStack.Text;

namespace Letterbook.Docs;

public class YouTubeInlineContainer : HtmlObjectRenderer<CustomContainerInline>
{
	protected override void Write(HtmlRenderer renderer, CustomContainerInline obj)
	{
		var youtubeId = obj.FirstChild is Markdig.Syntax.Inlines.LiteralInline literalInline
			? literalInline.Content.AsSpan().RightPart(' ').ToString()
			: null;
		if (string.IsNullOrEmpty(youtubeId))
		{
			renderer.WriteLine($"Missing YouTube Id, Usage ::YouTube <id>::");
			return;
		}
		renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <lite-youtube class=""w-full mx-4 my-4"" width=""560"" height=""315"" videoid=""{youtubeId}""
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')""></lite-youtube>
        </div>");
	}
}
