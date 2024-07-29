using Microsoft.Extensions.FileProviders;

namespace Letterbook.DocsSsg.Files;

public static class Extensions
{
	private static FileInfo FileInfoFromPath(string f) => new()
	{
		Exists = File.Exists(f),
		IsDirectory = false,
		LastModified = File.GetLastWriteTime(f),
		Name = Path.GetFileName(f),
		PhysicalPath = f
	};

	private static FileInfo DirInfoFromPath(string f) => new()
	{
		Exists = Directory.Exists(f),
		IsDirectory = true,
		LastModified = Directory.GetLastWriteTime(f),
		Name = Path.GetFileName(f.TrimEnd('/')),
		PhysicalPath = f
	};

	public static IEnumerable<IFileInfo> GetFiles(this IEnumerable<IFileInfo> dir) =>
		dir.SelectMany(d => Directory.GetFiles(d.PhysicalPath!)).Select(FileInfoFromPath);

	public static IEnumerable<IFileInfo> GetDirectories(this IEnumerable<IFileInfo> dir) =>
		dir.SelectMany(d => Directory.GetDirectories(d.PhysicalPath!)).Select(DirInfoFromPath);
}