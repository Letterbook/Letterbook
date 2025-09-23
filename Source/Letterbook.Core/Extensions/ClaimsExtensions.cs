using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.Core.Extensions;

public static class ClaimsExtensions
{
	public static bool TryGetAccountId(this IEnumerable<Claim> claims, out Guid id)
	{
		return Guid.TryParse(claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value, out id);
	}
}