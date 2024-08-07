using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack.IO;
using ServiceStack.Text;

namespace Letterbook.Docs;

// This won't work for all Mastodon servers, they must host an embed.js script to make it all work completely. iframe's may still function without that but in a limited state.
public class MastodonInlineContainer : HtmlObjectRenderer<CustomContainerInline>
{
	protected override void Write(HtmlRenderer renderer, CustomContainerInline obj)
	{
		var postUrl = obj.FirstChild is Markdig.Syntax.Inlines.LiteralInline literalInline
			? literalInline.Content.AsSpan().RightPart(' ').ToString()
			: null;
		if (string.IsNullOrEmpty(postUrl))
		{
			renderer.WriteLine($"Missing YouTube Id, Usage ::YouTube <id>::");
			return;
		}
		var embedPathUrl = $"https://{new Uri(postUrl).Host}/embed.js";

		renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <iframe src=""{postUrl}/embed"" class=""mastodon-embed"" style=""max-width: 100%; border: 0"" width=""600"" allowfullscreen=""allowfullscreen""></iframe><script src=""{embedPathUrl}"" async=""async""></script>
            </div>
        </div>");
	}
}
