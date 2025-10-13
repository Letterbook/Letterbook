using System.Security.Claims;
using Letterbook.Api.Authentication.HttpSignature.Infrastructure;
using Letterbook.Core;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Letterbook.AspNet;

public class AuthorizePeerService: Attribute, IAsyncActionFilter
{
	private readonly IModerationService _moderation;

	public AuthorizePeerService(IModerationService moderation)
	{
		_moderation = moderation;
	}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (context.HttpContext.Features.Get<HttpSignatureFeature>() is not { } signatures)
		{
			await next();
			return;
		}
		var svc = _moderation.As(context.HttpContext.User.Claims);

		foreach (var signature in signatures.GetValidatedSignatures())
		{
			var peerRestrictions = await svc.GetOrInitPeerRestrictions(signature);
			var peerClaims = peerRestrictions.Select(r => r.AsClaim()).ToList();
			var defederated = peerClaims.Any(predicate: claim =>
				claim.Type == RestrictionsExtensions.RestrictionType && Enum.TryParse<Restrictions>(claim.Value, out var r) &&
				r == Restrictions.Defederate);
			if (defederated)
			{
				context.Result = new ForbidResult();
			}
			context.HttpContext.User.AddIdentity(new ClaimsIdentity(peerClaims));
		}

		await next();
	}
}