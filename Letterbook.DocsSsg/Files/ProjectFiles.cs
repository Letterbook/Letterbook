using Microsoft.Extensions.FileProviders;

namespace Letterbook.DocsSsg.Files;

public class ProjectFiles(IWebHostEnvironment environment) : IProjectFiles
{
	private readonly IFileProvider _root = environment.ContentRootFileProvider;

	public IEnumerable<IFileInfo> GetSubdirectories(string path) => _root.GetDirectoryContents(path).Where(c => c.IsDirectory);
	public IEnumerable<IFileInfo> GetFiles(string path) => _root.GetDirectoryContents(path).Where(c => !c.IsDirectory);

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
}