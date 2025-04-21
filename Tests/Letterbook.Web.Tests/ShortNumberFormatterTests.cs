using System.Globalization;

namespace Letterbook.Web.Tests;

public class ShortNumberFormatterTests
{
	[InlineData(0, "0")]
	[InlineData(50, "50")]
	[InlineData(999, "999")]
	[InlineData(1_000, "1.0K")]
	[InlineData(1_100, "1.1K")]
	[InlineData(1_101, "1.1K")]
	[InlineData(11_100, "11.1K")]
	[InlineData(1_110_000, "1.1M")]
	[InlineData(121_110_000, "121.1M")]
	[InlineData(2_111_100_000, "2.1B")]
	[Theory]
	public void CanFormat(int given, string expected)
	{
		CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
		var actual = string.Format(ShortNumberFormatter.Instance, "{0:compact}", given);
		Assert.Equal(expected, actual);
	}

	[InlineData("us", "123.4K")]
	[InlineData("cn", "123.4K")]
	[InlineData("uk", "123,4K")]
	[InlineData("fr", "123,4K")]
	[InlineData("de", "123,4K")]
	[InlineData("es", "123,4K")]
	[InlineData("ca", "123,4K")]
	[InlineData("in", "123,4K")]
	[InlineData("ru", "123,4K")]
	[InlineData("jp", "123.4K")]
	[Theory]
	public void CanFormatCulture(string culture, string expected)
	{
		CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
		var actual = string.Format(ShortNumberFormatter.Instance, "{0:compact}", 123400);
		Assert.Equal(expected, actual);
	}
}