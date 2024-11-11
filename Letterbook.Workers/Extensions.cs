using System.Security.Claims;

namespace Letterbook.Workers;

public static class Extensions
{
	public static Claim MapClaim(Contracts.Claim c) => (Claim)c;
}