namespace Letterbook.Docs.Markdown;

public interface IMarkdownFiles
{
	List<T> GetAll<T>() where T : MarkdownDoc;
	void LoadFrom(string dir);

	MarkdownDoc Reload(MarkdownDoc doc);
}