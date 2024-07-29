using Microsoft.Extensions.FileProviders;

namespace Letterbook.DocsSsg.Files;

public static class Extensions
{
	public static IEnumerable<IFileInfo> GetFiles(this IEnumerable<IFileInfo> dir) => dir.Where(info => !info.IsDirectory);

	public static IEnumerable<string> GetPaths(this IEnumerable<IFileInfo> files) =>
		files.Where(info => info.PhysicalPath != null).Select(info => info.PhysicalPath!);
}