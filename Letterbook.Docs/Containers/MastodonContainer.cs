using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Letterbook.Docs;

// This won't work for all Mastodon servers, they must host an embed.js script to make it all work completely. iframe's may still function without that but in a limited state.
public class MastodonContainer : HtmlObjectRenderer<CustomContainer>
{
	protected override void Write(HtmlRenderer renderer, CustomContainer obj)
	{
		if (obj.Arguments == null)
		{
			renderer.WriteLine($"Missing Mastodon Post, Usage :::{obj.Info} <postUrl>");
			return;
		}

		renderer.EnsureLine();

		var postUrl = obj.Arguments!;
		var embedPathUrl = $"https://{new Uri(postUrl).Host}/embed.js";
		var attrs = obj.TryGetAttributes()!;
		attrs.Classes ??= new();
		attrs.Classes.Add("not-prose justify-center");

		renderer.Write("<div").WriteAttributes(obj).Write('>');
		renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <iframe src=""{postUrl}/embed"" class=""mastodon-embed"" style=""max-width: 100%; border: 0"" width=""600"" allowfullscreen=""allowfullscreen""></iframe><script src=""{embedPathUrl}"" async=""async""></script>
            </div>
        </div>");
	}
}