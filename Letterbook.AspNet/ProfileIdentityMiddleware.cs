using System.Security.Claims;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.AspNet;

public class ProfileIdentityMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ProfileIdentityMiddleware> _logger;

	public ProfileIdentityMiddleware(RequestDelegate next, ILogger<ProfileIdentityMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context, IAccountService accounts)
	{
		_logger.LogDebug("Middleware: {Name}", GetType().Name);
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
		    await accounts.LookupAccount(accountId) is not { } account)
		{
			if (context.Response.HasStarted) return;

			await context.ChallengeAsync(context.User.Identity.AuthenticationType);
			_logger.LogInformation("Middleware: {Name} challenged {AuthenticationScheme}", GetType().Name,
				context.User.Identity.AuthenticationType);
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
				_logger.LogWarning("Middleware: {Name} invalid claim {Claim}", GetType().Name,
					ApplicationClaims.ActiveProfile);
				if (context.Response.HasStarted) return;

				await context.ChallengeAsync(context.User.Identity.AuthenticationType);
				_logger.LogInformation("Middleware: {Name} challenged {AuthenticationScheme}", GetType().Name,
					context.User.Identity.AuthenticationType);
				return;
			}
		}

		// 4 - Build the profile claims identity and add it to the User principal
		var claimsIdentity = new ClaimsIdentity(account.ProfileClaims(!hasActiveProfile), context.User.Identity?.AuthenticationType);
		context.User.AddIdentity(claimsIdentity);
		_logger.LogDebug("Middleware: {Name} complete", GetType().Name);

		await _next(context);
	}
}