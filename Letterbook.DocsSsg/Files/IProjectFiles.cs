using Microsoft.Extensions.FileProviders;

namespace Letterbook.DocsSsg.Files;

public interface IProjectFiles
{
	public IEnumerable<IFileInfo> GetSubdirectories(string path);
	public IEnumerable<IFileInfo> GetFiles(string path);
	public void ClearDist();
	public Task WriteToDist(string path, Stream stream);
}