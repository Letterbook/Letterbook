using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Letterbook.Docs.Markdown;

public class ContainerExtensions : IMarkdownExtension
{
	public static ContainerExtensions Instance = new();

	public Dictionary<string, Func<CodeBlockRenderer?, HtmlObjectRenderer<CodeBlock>>> CodeBlocks { get; set; } = new();
	public Dictionary<string, HtmlObjectRenderer<CustomContainer>> BlockContainers { get; set; } = new();
	public Dictionary<string, HtmlObjectRenderer<CustomContainerInline>> InlineContainers { get; set; } = new();

	private ContainerExtensions() { }

	public void AddCodeBlock(string name, Func<CodeBlockRenderer?, HtmlObjectRenderer<CodeBlock>> fenceCodeBlock) =>
		CodeBlocks[name] = fenceCodeBlock;

	public void AddBlockContainer(string name, HtmlObjectRenderer<CustomContainer> container) =>
		BlockContainers[name] = container;

	public void AddInlineContainer(string name, HtmlObjectRenderer<CustomContainerInline> container) =>
		InlineContainers[name] = container;

	public void AddBuiltInContainers(string[]? exclude = null)
	{
		CodeBlocks = new()
		{
			["files"] = origRenderer => new FilesCodeBlockRenderer(origRenderer)
		};
		BlockContainers = new()
		{
			["tip"] = new CustomInfoRenderer(),
			["info"] = new CustomInfoRenderer
			{
				Class = "info",
				Title = "INFO",
			},
			["warning"] = new CustomInfoRenderer
			{
				Class = "warning",
				Title = "WARNING",
			},
			["danger"] = new CustomInfoRenderer
			{
				Class = "danger",
				Title = "DANGER",
			},
			["youtube"] = new YouTubeContainerRenderer(),
		};
		InlineContainers = new()
		{
			["youtube"] = new YouTubeContainerInlineRenderer(),
		};

		if (exclude != null)
		{
			foreach (var name in exclude)
			{
				BlockContainers.TryRemove(name, out _);
				InlineContainers.TryRemove(name, out _);
			}
		}
	}

	public void Setup(MarkdownPipelineBuilder pipeline)
	{
		if (!pipeline.BlockParsers.Contains<CustomContainerParser>())
		{
			// Insert the parser before any other parsers
			pipeline.BlockParsers.Insert(0, new CustomContainerParser());
		}

		// Plug the inline parser for CustomContainerInline
		var inlineParser = pipeline.InlineParsers.Find<EmphasisInlineParser>();
		if (inlineParser != null && !inlineParser.HasEmphasisChar(':'))
		{
			inlineParser.EmphasisDescriptors.Add(new EmphasisDescriptor(':', 2, 2, true));
			inlineParser.TryCreateEmphasisInlineList.Add((emphasisChar, delimiterCount) =>
			{
				if (delimiterCount >= 2 && emphasisChar == ':')
				{
					return new CustomContainerInline();
				}

				return null;
			});
		}
	}

	public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
	{
		if (renderer is HtmlRenderer htmlRenderer)
		{
			if (!htmlRenderer.ObjectRenderers.Contains<CustomContainerRenderers>())
			{
				// Must be inserted before CodeBlockRenderer
				htmlRenderer.ObjectRenderers.Insert(0, new CustomContainerRenderers(this));
			}

			// htmlRenderer.ObjectRenderers.TryRemove<HtmlCustomContainerInlineRenderer>();
			// Must be inserted before EmphasisRenderer
			// htmlRenderer.ObjectRenderers.Insert(0, new CustomContainerInlineRenderers(this));
		}
	}
}