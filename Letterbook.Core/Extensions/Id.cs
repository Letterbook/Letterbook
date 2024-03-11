using Medo;

namespace Letterbook.Core.Extensions;

public static class Id
{
	public static bool TryAsUuid7(string id, out Uuid7 uuid)
	{
		try
		{
			uuid = Uuid7.FromId25String(id);
			return true;
		}
		catch (Exception)
		{
			uuid = Uuid7.Empty;
			return false;
		}
	}
}