using System.Security.Claims;
using System.Text.Encodings.Web;
using Letterbook.Api.Authentication.HttpSignature.Infrastructure;
using Letterbook.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Api.Authentication.HttpSignature.Handler;

public class HttpSignatureAuthenticationHandler : AuthenticationHandler<HttpSignatureAuthenticationOptions>
{
	private readonly ILogger<HttpSignatureAuthenticationHandler> _logger;

	public HttpSignatureAuthenticationHandler(
		IOptionsMonitor<HttpSignatureAuthenticationOptions> options,
		ILoggerFactory loggerFactory,
		UrlEncoder encoder)
		: base(options, loggerFactory, encoder)
	{
		_logger = loggerFactory.CreateLogger<HttpSignatureAuthenticationHandler>();
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		try
		{
			var signatureFeature = Context.Features.Get<HttpSignatureFeature>();
			if (signatureFeature == null)
			{
				_logger.LogError($"Ensure that {nameof(HttpSignatureVerificationMiddleware)} is added to the pipeline before authentication.");
				return Task.FromResult(AuthenticateResult.Fail("HTTP Signature feature is not available in the request context."));
			}

			var identities = signatureFeature
				.GetValidatedSignatures()
				.Select(i =>
					new ClaimsIdentity(new[]
					{
						new Claim(ApplicationClaims.Actor, i.ToString())
					}, HttpSignatureAuthenticationDefaults.Scheme)
				);

			var principal = new ClaimsPrincipal(identities);
			if (principal.Identities.Any())
			{
				_logger.LogInformation("Successfully authenticated: {IdentityList}", string.Join(", ", principal.Identities.Select(i => i.Name)));
				return Task.FromResult(AuthenticateResult.Success(
					new AuthenticationTicket(
						principal,
						HttpSignatureAuthenticationDefaults.Scheme)));
			}
		}
		catch (Exception ex)
		{
			_logger.LogError("Unhandled exception during request signature authentication: {Exception}", ex);
		}

		return Task.FromResult(AuthenticateResult.Fail("No valid HTTP signatures found"));
	}
}