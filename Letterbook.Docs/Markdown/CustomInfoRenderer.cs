using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Letterbook.Docs.Markdown;

public class CustomInfoRenderer : HtmlObjectRenderer<CustomContainer>
{
	public string Title { get; set; } = "TIP";
	public string Class { get; set; } = "tip";

	protected override void Write(HtmlRenderer renderer, CustomContainer obj)
	{
		renderer.EnsureLine();
		if (renderer.EnableHtmlForBlock)
		{
			var title = obj.Arguments ?? obj.Info;
			if (string.IsNullOrEmpty(title))
				title = Title;
			renderer.Write(@$"<div class=""{Class} custom-block"">
            <p class=""custom-block-title"">{title}</p>");
		}

		// We don't escape a CustomContainer
		renderer.WriteChildren(obj);
		if (renderer.EnableHtmlForBlock)
		{
			renderer.WriteLine("</div>");
		}
	}
}