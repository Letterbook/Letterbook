using System.Net;
using System.Security.Claims;
using Letterbook.Core;
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

		var cancellationToken = context.RequestAborted;

		// 2 - Lookup the account with LinkedProfiles
		var userSub = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
		if (string.IsNullOrEmpty(userSub) ||
		    !Guid.TryParse(userSub, out var accountId) ||
		    await accounts.LookupAccount(accountId) is not {} account)
		{
			if (context.Response.HasStarted) return;

			context.Response.Clear();
			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			await context.Response.WriteAsync("Required sub claim is invalid", cancellationToken);
			return;
		}

		var hasActiveProfile = context.User.Claims.Any(claim => claim.Type == ApplicationClaims.ActiveProfile);

		// 3 - Build the profile claims identity and add it to the User principal
		var claimsIdentity = new ClaimsIdentity(account.ProfileClaims(!hasActiveProfile), context.User.Identity?.AuthenticationType);
		context.User.AddIdentity(claimsIdentity);
		await _next(context);
	}
}