using System.Text.RegularExpressions;
using Markdig;
using Markdig.Syntax;
using Microsoft.Extensions.FileProviders;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Letterbook.Docs.Markdown;

public abstract partial class MarkdownBase<T>(IWebHostEnvironment env, MarkdownPipeline pipeline) where T : MarkdownDoc
{
	[GeneratedRegex(@"(?<!^)(?=[A-Z\s\d])")]
	private static partial Regex SlugRegex();

	public virtual string Slug(string filename)
	{
		return string.Join('-', SlugRegex().Split(filename).Where(static s => s != ""));
	}
	public virtual T? Load(IFileInfo file)
	{
		if (Path.GetFileNameWithoutExtension(file.PhysicalPath) is not { } filename
		    || Path.GetExtension(file.PhysicalPath) != ".md"
		    || File.ReadAllText(file.PhysicalPath!) is not { } content)
			return default;
		var doc = CreateDocument(content);

		doc.Title ??= filename;
		doc.Path = file.PhysicalPath!;
		doc.FileName = Path.GetFileName(file.Name) ;
		doc.Slug = Slug(filename);

		return doc;
	}

	public abstract MarkdownDoc Reload(MarkdownDoc doc);
	public virtual T CreateDocument(string content)
	{
		var writer = new StringWriter();
		var ledeWriter = new StringWriter();
		var renderer = new Markdig.Renderers.HtmlRenderer(writer);
		var ledeRenderer = new Markdig.Renderers.HtmlRenderer(ledeWriter);
		pipeline.Setup(renderer);

		var document = Markdig.Markdown.Parse(content, pipeline);
		renderer.Render(document);

		var fmText = document.Descendants<Markdig.Extensions.Yaml.YamlFrontMatterBlock>().FirstOrDefault();
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();
		var doc = fmText is null
			? Activator.CreateInstance<T>()
			: deserializer.Deserialize<T>(string.Join('\n', fmText.Lines));

		if (document.Descendants<ParagraphBlock>().FirstOrDefault() is { } paragraph)
			ledeRenderer.Render(paragraph);

		doc.HtmlLede = ledeWriter.ToString();
		doc.Html = writer.ToString();
		doc.Source = content;
		return doc;
	}

	public bool IsVisible(MarkdownDoc doc) => env.IsDevelopment() || (!doc.Draft && doc.Date >= DateTime.UtcNow);

}