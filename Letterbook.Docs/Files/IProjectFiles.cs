using Letterbook.Docs.Markdown;
using Microsoft.Extensions.FileProviders;

namespace Letterbook.Docs.Files;

public interface IProjectFiles
{
	public IFileInfo? GetDirectory(string path);

	/// <summary>
	/// Get all the immediate subdirectories of the path
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public IEnumerable<IFileInfo> GetSubdirectories(string path);
	public IEnumerable<IFileInfo> GetSubdirectories(IFileInfo path);
	public IEnumerable<IFileInfo> GetSubdirectories(IEnumerable<IFileInfo> path);

	/// <summary>
	/// Get all the files immediately under the path
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public IEnumerable<IFileInfo> GetFiles(string path);
	public IEnumerable<IFileInfo> GetFiles(IFileInfo path);
	public IEnumerable<IFileInfo> GetFiles(IEnumerable<IFileInfo> path);

	/// <summary>
	/// Get the source project file backing the doc
	/// </summary>
	/// <param name="doc"></param>
	/// <returns></returns>
	public IFileInfo GetMarkdownDoc(MarkdownDoc doc);

	/// <summary>
	/// Get the relative project path for the file
	/// </summary>
	/// <param name="file"></param>
	/// <returns></returns>
	public string? GetPath(IFileInfo file);
	public void ClearDist();
	public void WriteToDist(string path, TextWriter writer);
}