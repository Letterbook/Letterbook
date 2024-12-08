namespace Letterbook.Docs.Markdown;

public class LoaderFactory(IServiceProvider services) : IMarkdownMeta
{
	private readonly Lazy<Dictionary<string, IMarkdownFiles>> _map = new();
	private Dictionary<string, IMarkdownFiles> Map => _map.Value;

	/// <summary>
	/// Get an IMarkdownFiles instance that loads from the given source
	/// </summary>
	/// <typeparam name="TLoader">Markdown loader for the file layout on disk</typeparam>
	/// <typeparam name="TDoc">Markdown doc type to be loaded</typeparam>
	/// <param name="source">A project directory source for loading Markdown files</param>
	/// <example>factory.GetMarkdown("_blog")</example>
	/// <returns></returns>
	public TLoader LoadFrom<TLoader, TDoc>(string source)
		where TLoader : IMarkdownFiles
		where TDoc : MarkdownDoc
	{
		if (Map.TryGetValue(source, out var value))
		{
			return (TLoader)value;
		}

		value = services.GetRequiredService<TLoader>();
		value.LoadFrom<TDoc>(source);
		Map.Add(source, value);

		return (TLoader)value;
	}
}