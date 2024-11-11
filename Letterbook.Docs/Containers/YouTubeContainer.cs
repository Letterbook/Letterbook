using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Letterbook.Docs;

public class YouTubeContainer : HtmlObjectRenderer<CustomContainer>
{
	protected override void Write(HtmlRenderer renderer, CustomContainer obj)
	{
		if (obj.Arguments == null)
		{
			renderer.WriteLine($"Missing YouTube Id, Usage :::{obj.Info} <id>");
			return;
		}

		renderer.EnsureLine();

		var youtubeId = obj.Arguments!;
		var attrs = obj.TryGetAttributes()!;
		attrs.Classes ??= new();
		attrs.Classes.Add("not-prose justify-center");

		renderer.Write("<div").WriteAttributes(obj).Write('>');
		renderer.WriteLine("<div class=\"text-3xl font-extrabold tracking-tight\">");
		renderer.WriteChildren(obj);
		renderer.WriteLine("</div>");
		renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <lite-youtube class=""w-full mx-4 my-4"" width=""560"" height=""315"" videoid=""{youtubeId}""
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')""></lite-youtube>
            </div>
        </div>");
	}
}