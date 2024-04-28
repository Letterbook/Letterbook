using Letterbook.Api.Authentication.HttpSignature.Handler;
using Letterbook.Api.Authentication.HttpSignature.Infrastructure;
using Letterbook.Api.Authentication.HttpSignature.Verification;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Api.Authentication.HttpSignature.DependencyInjection;

public static class HttpSignatureAuthenticationRegistrationExtensions
{
	public static AuthenticationBuilder AddHttpSignature(this AuthenticationBuilder builder)
	{

		builder.Services.AddScoped<IFederatedActorHttpSignatureVerifier, FederatedActorHttpSignatureVerifier>();
		builder.Services.AddScoped<HttpSignatureVerificationMiddleware>();

		return builder.AddScheme<HttpSignatureAuthenticationOptions, HttpSignatureAuthenticationHandler>(
			HttpSignatureAuthenticationDefaults.Scheme,
			static _ =>
			{

			});
	}

	public static IApplicationBuilder UseHttpSignatureVerification(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<HttpSignatureVerificationMiddleware>();
	}
}