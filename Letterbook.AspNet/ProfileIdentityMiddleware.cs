using System.Security.Claims;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.AspNet;

public class ProfileIdentityMiddleware
{
	private readonly RequestDelegate _next;

	public ProfileIdentityMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context, IAccountService accounts)
	{
		// 1 - if the request is not authenticated, nothing to do
		if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
		{
			await _next(context);
			return;
		}

		// 2 - Lookup the account with LinkedProfiles
		var userSub = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
		if (string.IsNullOrEmpty(userSub) ||
		    !Guid.TryParse(userSub, out var accountId) ||
		    await accounts.LookupAccount(accountId) is not {} account)
		{
			if (context.Response.HasStarted) return;

			await context.ChallengeAsync(context.User.Identity.AuthenticationType);
			return;
		}

		// 3 - Validate permission on the claimed activeProfile
		var activeProfile = context.User.Claims.FirstOrDefault(claim => claim.Type == ApplicationClaims.ActiveProfile);
		var hasActiveProfile = activeProfile != null;
		if (hasActiveProfile)
		{
			var valid = account.LinkedProfiles.Any(link => link.Profile.GetId25() == activeProfile!.Value);
			if (!valid)
			{
				if (context.Response.HasStarted) return;

				await context.ChallengeAsync(context.User.Identity.AuthenticationType);
				return;
			}
		}

		// 4 - Build the profile claims identity and add it to the User principal
		var claimsIdentity = new ClaimsIdentity(account.ProfileClaims(!hasActiveProfile), context.User.Identity?.AuthenticationType);
		context.User.AddIdentity(claimsIdentity);
		await _next(context);
	}
}