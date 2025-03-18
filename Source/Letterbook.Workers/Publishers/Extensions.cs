using MassTransit;
using Claim = System.Security.Claims.Claim;

namespace Letterbook.Workers.Publishers;

public static class Extensions
{
	/// <summary>
	/// Set default custom headers for the event
	/// </summary>
	/// <param name="context"></param>
	/// <param name="eventType"></param>
	public static void SetCustomHeaders<T>(this PublishContext<T> context, string eventType) where T : class
	{
		context.Headers.Set(Headers.Event, eventType);
	}

	/// <summary>
	/// Map Claims to serializeble DTO objects
	/// </summary>
	/// <param name="claims"></param>
	/// <returns></returns>
	public static Contracts.Claim[] MapDto(this IEnumerable<Claim> claims)
	{
		return claims.Select(c => (Contracts.Claim)c).ToArray();
	}
}