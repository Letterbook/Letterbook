using System.Text.RegularExpressions;
using Markdig;
using Markdig.Syntax;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Letterbook.DocsSsg.Markdown;

public abstract partial class MarkdownBase<T>(IWebHostEnvironment env, MarkdownPipeline pipeline) where T : MarkdownDoc
{
	[GeneratedRegex(@"(?<!^)(?=[A-Z\s\d])")]
	private static partial Regex SlugRegex();

	public virtual string Slug(string filename)
	{
		return string.Join('-', SlugRegex().Split(filename).Where(static s => s != ""));
	}
	public virtual T? Load(string path)
	{
		if (Path.GetFileNameWithoutExtension(path) is not { } filename || File.ReadAllText(path) is not { } content)
			return default;
		var doc = CreateDocument(content);

		doc.Title ??= filename;

		doc.Path = path;
		doc.FileName = Path.GetFileName(path) ;
		doc.Slug = Slug(filename);

		return doc;
	}

	public virtual T CreateDocument(string content)
	{
		var renderer = new Markdig.Renderers.HtmlRenderer(new StringWriter());
		pipeline.Setup(renderer);

		var document = Markdig.Markdown.Parse(content, pipeline);
		renderer.Render(document);

		var fmText = document.Descendants<Markdig.Extensions.Yaml.YamlFrontMatterBlock>().FirstOrDefault();
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance)
			.Build();
		var doc = fmText is null
			? Activator.CreateInstance<T>()
			: deserializer.Deserialize<T>(string.Join('\n', fmText.Lines.Lines).Trim(['-']));


		return doc;
	}

	public bool IsVisible(MarkdownDoc doc) => env.IsDevelopment() || (!doc.Draft && doc.Date >= DateTime.UtcNow);

}