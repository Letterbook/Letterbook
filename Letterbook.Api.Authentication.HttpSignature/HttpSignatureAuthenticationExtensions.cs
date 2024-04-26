using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Api.Authentication.HttpSignature;

public static class HttpSignatureAuthenticationExtensions
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