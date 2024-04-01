using System.Reflection;

namespace Letterbook.Adapter.ActivityPub.Test;

public static class TestDataHelpers
{
	private static string DataDir => Path.Join(
		Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");

	public static Stream ReadTestData(string fileName)
	{
		return new FileStream(Path.Join(DataDir, fileName), FileMode.Open);
	}
}