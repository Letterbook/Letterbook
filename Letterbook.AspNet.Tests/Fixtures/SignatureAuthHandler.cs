using System.Security.Claims;
using System.Text.Encodings.Web;
using Letterbook.Api.Authentication.HttpSignature.Handler;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.AspNet.Tests.Fixtures;

public class SignatureAuthHandler : AuthenticationHandler<HttpSignatureAuthenticationOptions>
{
	public SignatureAuthHandler(IOptionsMonitor<HttpSignatureAuthenticationOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, TimeProvider clock)
		: base(options, logger, encoder)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!Context.Request.Headers.TryGetValue("Authorization", out var authz))
			return Task.FromResult(AuthenticateResult.NoResult());
		var parts = authz[0]!.Split(' ');
		if (parts[0] != "Signed")
			return Task.FromResult(AuthenticateResult.NoResult());
		var claims = new[]
		{
			new Claim(ApplicationClaims.Actor, parts[1]!)
		};
		var identity = new ClaimsIdentity(claims, "Test");
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, "Test");

		var result = AuthenticateResult.Success(ticket);

		return Task.FromResult(result);
	}
}