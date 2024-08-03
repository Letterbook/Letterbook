namespace Letterbook.Docs.Markdown;

public class MarkdownFilesFactory<T>(IServiceProvider services) : IMarkdownMeta where T : IMarkdownFiles
{
	private readonly Lazy<Dictionary<string, T>> _map = new();
	private Dictionary<string, T> Map => _map.Value;

	/// <summary>
	/// Get an IMarkdownFiles instance that loads from the given source
	/// </summary>
	/// <typeparam name="T">IMarkdownFiles implementation</typeparam>
	/// <param name="source">A project directory source for loading Markdown files</param>
	/// <example>factory.GetMarkdown("_blog")</example>
	/// <returns></returns>
	public T GetMarkdown(string source)
	{
		if (Map.TryGetValue(source, out var value))
		{
			return value;
		}

		value = services.GetRequiredService<T>();
		value.LoadFrom(source);
		Map.Add(source, value);

		return value;
	}
}