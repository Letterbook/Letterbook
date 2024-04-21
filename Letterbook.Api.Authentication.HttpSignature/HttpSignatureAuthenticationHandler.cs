using System.Security.Claims;
using System.Text.Encodings.Web;
using Letterbook.Adapter.ActivityPub.Signatures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Api.Authentication.HttpSignature;

public class HttpSignatureAuthenticationHandler : AuthenticationHandler<HttpSignatureAuthenticationOptions>
{
	private readonly IFederatedActorHttpSignatureVerifier _signatureVerifier;
	private readonly ILogger<HttpSignatureAuthenticationHandler> _logger;

	public HttpSignatureAuthenticationHandler(
		IOptionsMonitor<HttpSignatureAuthenticationOptions> options,
		ILoggerFactory loggerFactory,
		IFederatedActorHttpSignatureVerifier signatureVerifier,
		UrlEncoder encoder)
		: base(options, loggerFactory, encoder)
	{
		_signatureVerifier = signatureVerifier;
		_logger = loggerFactory.CreateLogger<HttpSignatureAuthenticationHandler>();
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		try
		{
			var principal = new ClaimsPrincipal();
			await foreach (var verifiedIdentity in _signatureVerifier.VerifyAsync(Context, Context.RequestAborted))
			{
				var identity = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, verifiedIdentity.ToString()) { }
				}, HttpSignatureAuthenticationDefaults.Scheme);

				principal.AddIdentity(identity);
			}

			if (principal.Identities.Any())
			{
				_logger.LogInformation("Successfully authenticated: {IdentityList}", string.Join(", ", principal.Identities.Select(i => i.Name)));
				return AuthenticateResult.Success(
					new AuthenticationTicket(
						principal,
						HttpSignatureAuthenticationDefaults.Scheme));
			}
		}
		catch (Exception ex)
		{
			_logger.LogError("Unhandled exception during request signature authentication: {Exception}", ex);
		}

		return AuthenticateResult.Fail("No valid HTTP signatures found");
	}
}

public class HttpSignatureAuthenticationOptions : AuthenticationSchemeOptions
{
}

public static class HttpSignatureAuthenticationDefaults
{
	public static readonly string Scheme = "HttpSignature";
}

public static class HttpSignatureAuthenticationExtensions
{
	public static AuthenticationBuilder AddHttpSignature(this AuthenticationBuilder builder)
	{
		return builder.AddScheme<HttpSignatureAuthenticationOptions, HttpSignatureAuthenticationHandler>(
			HttpSignatureAuthenticationDefaults.Scheme,
			static _ =>
			{

			});
	}
}