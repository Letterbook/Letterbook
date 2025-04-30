using System.Globalization;

namespace Letterbook.Web;

public class ShortNumberFormatter : ICustomFormatter, IFormatProvider
{
	public static ShortNumberFormatter Instance = new();
	public string Format(string? format, object? arg, IFormatProvider? formatProvider)
	{
		if (arg is not int value)
			return "";
		if (format != "compact")
			return "";
		return value switch
		{
			>= 1_000_000_000 => (value / 1_000_000_000m).ToString("0.0B", CultureInfo.CurrentCulture),
			>= 1_000_000 => (value / 1_000_000m).ToString("0.0M", CultureInfo.CurrentCulture),
			>= 1_000 => (value / 1_000m).ToString("0.0K", CultureInfo.CurrentCulture),
			_ => value.ToString()
		};
	}

	public object? GetFormat(Type? formatType)
	{
		if (formatType == typeof(ICustomFormatter))
			return this;
		else
			return null;
	}
}