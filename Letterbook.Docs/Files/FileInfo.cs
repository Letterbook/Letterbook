using Microsoft.Extensions.FileProviders;

namespace Letterbook.Docs.Files;

public class FileInfo : IFileInfo
{
	public Stream CreateReadStream() => new FileStream(PhysicalPath!, FileMode.Open, FileAccess.Read, FileShare.Read);

	public bool Exists { get; init; }
	public bool IsDirectory { get; init; }
	public DateTimeOffset LastModified { get; init; }
	public long Length { get; init; }
	public required string Name { get; init; }
	public string? PhysicalPath { get; init; }
}
