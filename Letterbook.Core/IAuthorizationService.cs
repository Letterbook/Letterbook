using System.Security.Claims;
using Letterbook.Core.Authorization;
using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IAuthorizationService
{
	/// <summary>
	/// Authorize creating a specific resource
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="resource"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Decision Create<T>(IEnumerable<Claim> claims, T resource);

	/// <summary>
	/// Authorize creatine a new resource by type
	/// </summary>
	/// <see cref="Create{T}(System.Collections.Generic.IEnumerable{System.Security.Claims.Claim},T)"/>
	public Decision Create<T>(IEnumerable<Claim> claims);

	/// <summary>
	/// Authorize modifying an existing resource
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="resource"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Decision Update<T>(IEnumerable<Claim> claims, T resource);

	/// <summary>
	/// Authorize modifying a type of resource
	/// </summary>
	/// <see cref="Update{T}(System.Collections.Generic.IEnumerable{System.Security.Claims.Claim},T)"/>
	public Decision Update<T>(IEnumerable<Claim> claims);

	/// <summary>
	/// Authorize deleting an existing resource
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="resource"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Decision Delete<T>(IEnumerable<Claim> claims, T resource);

	/// <summary>
	/// Authorize adding a creator to a resource
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="resource"></param>
	/// <param name="attributeTo">The profile to add as creator</param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Decision Attribute<T>(IEnumerable<Claim> claims, T resource, ProfileId attributeTo);

	/// <summary>
	/// Authorize publishing a resource to its specified audience
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="resource"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Decision Publish<T>(IEnumerable<Claim> claims, T resource);

	/// <summary>
	/// Authorize reading or viewing a resource
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="resource"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Decision View<T>(IEnumerable<Claim> claims, T resource);

	/// <summary>Authorize reading or viewing a type of resource</summary>
	/// <see cref="View{T}(System.Collections.Generic.IEnumerable{System.Security.Claims.Claim},T)"/>
	public Decision View<T>(IEnumerable<Claim> claims);

	/// <summary>
	/// Authorize at least one claim on the Profile. This supports early exit for requests that are definitely unauthorized. In most cases
	/// some additional authorization will still be necessary.
	/// </summary>
	/// <param name="claims"></param>
	/// <param name="profileId"></param>
	/// <returns></returns>
	public Decision Any(IEnumerable<Claim> claims, ProfileId profileId);

	// TODO: collections/tags/whatever we call them
	// TODO: account stuff
	// TODO: profile stuff (notifications, follows, blocks, etc)
	// TODO: thread stuff (manage replies, maybe more)
}