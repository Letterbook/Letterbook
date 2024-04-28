using System.Reflection;

namespace Letterbook.Core.Tests;

public static class TestData
{
	private static string DataDir => Path.Join(
		Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");

	public static Stream Read(string fileName)
	{
		return new FileStream(Path.Join(DataDir, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
	}
}