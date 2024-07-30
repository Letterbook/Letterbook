using Microsoft.Extensions.FileProviders;

namespace Letterbook.DocsSsg.Files;

public static class Extensions
{
	public static IEnumerable<IFileInfo> Then(this IEnumerable<IFileInfo> files, Func<IEnumerable<IFileInfo>, IEnumerable<IFileInfo>> fn) =>
		fn(files);

	public static IEnumerable<IFileInfo> Then(this IFileInfo file, Func<IFileInfo, IEnumerable<IFileInfo>> fn) => fn(file);
}