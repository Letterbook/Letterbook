using System.Security.Claims;
using Letterbook.Core.Models;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.Core.Extensions;

public static class ClaimsExtensions
{
	public static bool TryGetAccountId(this IEnumerable<Claim> claims, out Guid id)
	{
		return Guid.TryParse(claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value, out id);
	}

	public static bool TryGetActiveProfileId(this IEnumerable<Claim> claims, out ProfileId id)
	{
		id = default;
		var activeProfile = claims.FirstOrDefault(c => c.Type == ApplicationClaims.ActiveProfile);
		if (activeProfile is not { } claim) return false;
		var result = ProfileId.TryParse(claim.Value, out var profileId);

		id = profileId;
		return result;
	}
}