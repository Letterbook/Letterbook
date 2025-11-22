using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Core.Authorization;

public class AuthorizationService : IAuthorizationService
{
	public Decision Create<T>(IEnumerable<Claim> claims, T target)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Create<T>(IEnumerable<Claim> claims)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Update<T>(IEnumerable<Claim> claims, T target)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Update<T>(IEnumerable<Claim> claims)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Delete<T>(IEnumerable<Claim> claims, T target)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Attribute<T>(IEnumerable<Claim> claims, T target, ProfileId attributeTo)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Publish<T>(IEnumerable<Claim> claims, T target)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Federate(IEnumerable<Claim> claims, IEnumerable<Restrictions> peerRestrictions)
	{
		if(peerRestrictions.Contains(Restrictions.Defederate))
			return Decision.Deny("defederated", claims);

		claims = claims.ToList();
		var builder = new DecisionBuilder(claims);
		foreach (var claim in claims.Where(claim => claim.Type == RestrictionsExtensions.RestrictionType))
		{
			if(!Enum.TryParse<Restrictions>(claim.Value, out var restriction))
				continue;
			if (restriction == Restrictions.Defederate)
			{
				builder.DisqualifiedBy(claim);
			}
		}

		return builder.Decide();
	}

	public Decision View<T>(IEnumerable<Claim> claims, T target)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision View<T>(IEnumerable<Claim> claims)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision List<T>(IEnumerable<Claim> claims)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Any(IEnumerable<Claim> claims, ProfileId profileId)
	{
		return Decision.Allow("todo", claims);
	}
}