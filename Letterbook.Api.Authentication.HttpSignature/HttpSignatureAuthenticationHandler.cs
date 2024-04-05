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

	public HttpSignatureAuthenticationHandler(
		IOptionsMonitor<HttpSignatureAuthenticationOptions> options,
		ILoggerFactory logger,
		IFederatedActorHttpSignatureVerifier signatureVerifier,
		UrlEncoder encoder)
		: base(options, logger, encoder)
	{
		_signatureVerifier = signatureVerifier;
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var isValid = await _signatureVerifier.VerifyAsync(Context, Context.RequestAborted);

		return AuthenticateResult.NoResult();
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