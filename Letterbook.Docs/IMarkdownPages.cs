namespace Letterbook.Docs;

public interface IMarkdownPages
{
	string Id { get; }
	List<MarkdownFileBase> GetAll();

	void LoadFrom(string dir);
}