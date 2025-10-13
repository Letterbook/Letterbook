namespace Letterbook.Core.Extensions;

public static class DateTimeExtensions
{
	public static bool Expired(this DateTimeOffset dt) => DateTimeOffset.UtcNow >= dt;
}