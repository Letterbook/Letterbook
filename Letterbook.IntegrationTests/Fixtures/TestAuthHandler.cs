using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Letterbook.IntegrationTests.Fixtures;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, TimeProvider clock)
		: base(options, logger, encoder)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		Context.Request.Headers.TryGetValue("Authorization", out var id);
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, id[0]!.Split(' ')[1]),// ?? Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.PreferredUsername, "Test user"),
			new Claim(JwtRegisteredClaimNames.Email, "Test@example.com"),
		};
		var identity = new ClaimsIdentity(claims, "Test");
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, "Test");

		var result = AuthenticateResult.Success(ticket);

		return Task.FromResult(result);
	}
}