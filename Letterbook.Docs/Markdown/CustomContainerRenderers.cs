using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Letterbook.Docs.Markdown;

public class CustomContainerRenderers(ContainerExtensions extensions) : HtmlObjectRenderer<CustomContainer>
{
	protected override void Write(HtmlRenderer renderer, CustomContainer obj)
	{
		var useRenderer = obj.Info != null && extensions.BlockContainers.TryGetValue(obj.Info, out var customRenderer)
			? customRenderer
			: new HtmlCustomContainerRenderer();
		useRenderer.Write(renderer, obj);
	}
}