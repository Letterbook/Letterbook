using Letterbook.DocsSsg.Markdown;
using Microsoft.Extensions.FileProviders;

namespace Letterbook.DocsSsg.Files;

public class ProjectFiles(IWebHostEnvironment environment) : IProjectFiles
{
	private readonly IFileProvider _root = environment.ContentRootFileProvider;
	private readonly string _rootPath = environment.ContentRootPath;

	public IFileInfo? GetDirectory(string path)
	{
		var p = _root.GetFileInfo(path);
		return p.IsDirectory ? p : default;
	}

	public IEnumerable<IFileInfo> GetSubdirectories(string path) =>
		_root.GetDirectoryContents(path).Where(IsDirectory);

	public IEnumerable<IFileInfo> GetSubdirectories(IFileInfo path) =>
		path.PhysicalPath == null ? [] : GetSubdirectories(Relative(path.PhysicalPath));

	public IEnumerable<IFileInfo> GetSubdirectories(IEnumerable<IFileInfo> path) =>
		path.SelectMany(GetSubdirectories);

	public IEnumerable<IFileInfo> GetFiles(string path) =>
		_root.GetDirectoryContents(path).Where(NotDirectory);

	public IEnumerable<IFileInfo> GetFiles(IFileInfo path) =>
		path.PhysicalPath == null ? [] : GetFiles(Relative(path.PhysicalPath));

	public IEnumerable<IFileInfo> GetFiles(IEnumerable<IFileInfo> path) =>
		path.SelectMany(GetFiles);

	public IFileInfo GetMarkdownDoc(MarkdownDoc doc) => _root.GetFileInfo(Relative(doc.Path));

	public void ClearDist()
	{
		if (_root.GetFileInfo("dist").PhysicalPath is not { } dest)
			throw new ProjectFilesException("Cannot clear dist directory");
		Directory.Delete(dest);
	}

	public Task WriteToDist(string path, Stream stream)
	{
		var dest = Path.Join(_root.GetFileInfo("dist").PhysicalPath, path);
		if (Path.GetDirectoryName(dest) is not { } dir)
			throw new ProjectFilesException($"Cannot create {path}");
		Directory.CreateDirectory(dir);
		using var fs = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
		return stream.CopyToAsync(fs);
	}

	private string Relative(string physicalPath) => Path.GetRelativePath(_rootPath, physicalPath);
	private static bool IsDirectory(IFileInfo file) => file.IsDirectory;
	private static bool NotDirectory(IFileInfo file) => !file.IsDirectory;
}