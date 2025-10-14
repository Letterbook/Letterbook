using System.Security.Claims;
using Letterbook.Core.Models;
using Medo;

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

	public Decision Federate(IEnumerable<Claim> claims, Uri peer)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision View<T>(IEnumerable<Claim> claims, T target)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision View<T>(IEnumerable<Claim> claims)
	{
		return Decision.Allow("todo", claims);
	}

	public Decision Any(IEnumerable<Claim> claims, ProfileId profileId)
	{
		return Decision.Allow("todo", claims);
	}
}