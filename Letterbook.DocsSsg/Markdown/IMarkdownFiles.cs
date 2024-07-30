namespace Letterbook.DocsSsg.Markdown;

public interface IMarkdownFiles
{
	List<MarkdownDoc> GetAll();
	void LoadFrom(string dir);

	MarkdownDoc Reload(MarkdownDoc doc);
}