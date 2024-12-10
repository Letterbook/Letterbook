namespace Letterbook.Docs.Markdown;

public interface IMarkdownFiles
{
	List<T> GetAll<T>() where T : MarkdownDoc;
	void LoadFrom<T>(string dir) where T : MarkdownDoc;
	T Reload<T>(T doc) where T : MarkdownDoc;
}