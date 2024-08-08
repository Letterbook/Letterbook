namespace Letterbook.Web.Tests.UI.Support;

public class SkipUnlessPortEquals : FactAttribute
{
	public SkipUnlessPortEquals(int port)
	{
		if (!Settings.NoSkip && port != Settings.BaseUrl.Port)
		{
			Skip = $"Skipped because test is not running against port <{port}> ({Settings.BaseUrl})";
		}
	}
}