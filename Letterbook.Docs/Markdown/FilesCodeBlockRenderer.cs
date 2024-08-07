using System.Text;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Letterbook.Docs.Markdown;

public class FilesCodeBlockRenderer(CodeBlockRenderer? underlyingRenderer = null) : HtmlObjectRenderer<CodeBlock>
{
	private readonly CodeBlockRenderer underlyingRenderer = underlyingRenderer ?? new CodeBlockRenderer();

	protected override void Write(HtmlRenderer renderer, CodeBlock obj)
	{
		if (obj is not FencedCodeBlock fencedCodeBlock || obj.Parser is not FencedCodeBlockParser parser)
		{
			underlyingRenderer.Write(renderer, obj);
			return;
		}

		var attributes = obj.TryGetAttributes() ?? new HtmlAttributes();
		var languageMoniker = fencedCodeBlock.Info?.Replace(parser.InfoPrefix!, string.Empty);
		if (string.IsNullOrEmpty(languageMoniker))
		{
			underlyingRenderer.Write(renderer, obj);
			return;
		}

		var txt = GetContent(obj);
		renderer
			.Write("<div")
			.WriteAttributes(attributes)
			.Write(">");

		var dir = ParseFileStructure(txt);
		RenderNode(renderer, dir);
		renderer.WriteLine("</div>");
	}

	private static string GetContent(LeafBlock obj)
	{
		var code = new StringBuilder();
		foreach (var line in obj.Lines.Lines)
		{
			var slice = line.Slice;
			if (slice.Text == null)
				continue;

			var lineText = slice.Text.Substring(slice.Start, slice.Length);
			code.AppendLine();
			code.Append(lineText);
		}

		return code.ToString();
	}

	public class Node
	{
		public List<string> Files { get; set; } = [];
		public Dictionary<string, Node> Directories { get; set; } = new();
	}

	public void RenderNode(HtmlRenderer html, Node model)
	{
		foreach (var (dirName, childNode) in model.Directories)
		{
			html.WriteLine("<div class=\"ml-6\">");
			html.WriteLine("  <div class=\"flex items-center text-base leading-8\">");
			html.WriteLine("    <svg class=\"mr-1 text-slate-600 inline-block select-none align-text-bottom overflow-visible\" aria-hidden=\"true\" focusable=\"false\" role=\"img\" viewBox=\"0 0 12 12\" width=\"12\" height=\"12\" fill=\"currentColor\"><path d=\"M6 8.825c-.2 0-.4-.1-.5-.2l-3.3-3.3c-.3-.3-.3-.8 0-1.1.3-.3.8-.3 1.1 0l2.7 2.7 2.7-2.7c.3-.3.8-.3 1.1 0 .3.3.3.8 0 1.1l-3.2 3.2c-.2.2-.4.3-.6.3Z\"></path></svg>");
			html.WriteLine("    <svg class=\"mr-1 text-sky-500\" aria-hidden=\"true\" focusable=\"false\" role=\"img\" viewBox=\"0 0 16 16\" width=\"16\" height=\"16\" fill=\"currentColor\"><path d=\"M.513 1.513A1.75 1.75 0 0 1 1.75 1h3.5c.55 0 1.07.26 1.4.7l.9 1.2a.25.25 0 0 0 .2.1H13a1 1 0 0 1 1 1v.5H2.75a.75.75 0 0 0 0 1.5h11.978a1 1 0 0 1 .994 1.117L15 13.25A1.75 1.75 0 0 1 13.25 15H1.75A1.75 1.75 0 0 1 0 13.25V2.75c0-.464.184-.91.513-1.237Z\"></path></svg>");
			html.WriteLine("    <span>" + dirName + "</span>");
			html.WriteLine("  </div>");
			RenderNode(html, childNode);
			html.WriteLine("</div>");
		}

		if (model.Files.Count > 0)
		{
			html.WriteLine("<div>");
			foreach (var file in model.Files)
			{
				html.WriteLine("<div class=\"ml-6 flex items-center text-base leading-8\">");
				html.WriteLine("  <svg class=\"mr-1 text-slate-600 inline-block select-none align-text-bottom overflow-visible\" aria-hidden=\"true\" focusable=\"false\" role=\"img\" viewBox=\"0 0 16 16\" width=\"16\" height=\"16\" fill=\"currentColor\"><path d=\"M2 1.75C2 .784 2.784 0 3.75 0h6.586c.464 0 .909.184 1.237.513l2.914 2.914c.329.328.513.773.513 1.237v9.586A1.75 1.75 0 0 1 13.25 16h-9.5A1.75 1.75 0 0 1 2 14.25Zm1.75-.25a.25.25 0 0 0-.25.25v12.5c0 .138.112.25.25.25h9.5a.25.25 0 0 0 .25-.25V6h-2.75A1.75 1.75 0 0 1 9 4.25V1.5Zm6.75.062V4.25c0 .138.112.25.25.25h2.688l-.011-.013-2.914-2.914-.013-.011Z\"></path></svg>");
				html.WriteLine("  <span>" + file + "</span>");
				html.WriteLine("</div>");
			}
			html.WriteLine("</div>");
		}
	}

	public static Node ParseFileStructure(string ascii, int indent = 2)
	{
		var lines = ascii.Trim().Split('\n').Where(x => x.Trim().Length > 0);
		var root = new Node();
		var stack = new Stack<Node>();
		stack.Push(root);

		foreach (var line in lines)
		{
			var depth = line.TakeWhile(char.IsWhiteSpace).Count() / indent;
			var name = line.Trim();
			var isDir = name.StartsWith('/');

			while (stack.Count > depth + 1)
				stack.Pop();

			var parent = stack.Peek();
			if (isDir)
			{
				var dirName = name.Substring(1);
				var dirContents = new Node();
				parent.Directories[dirName] = dirContents;
				stack.Push(dirContents);
			}
			else
			{
				parent.Files.Add(name);
			}
		}
		return root;
	}
}