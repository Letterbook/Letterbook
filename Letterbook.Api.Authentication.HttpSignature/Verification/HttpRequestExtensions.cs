using Microsoft.AspNetCore.Http;
using NSign;
using NSign.Signatures;

namespace Letterbook.Api.Authentication.HttpSignature.Verification;

/// <summary>
/// Helper methods for <see cref="AspNetCoreMessageSigningContext"/>. This code is copied and adapted from the NSign implementation
/// for <see cref="HttpRequestMessage"/>.
/// </summary>
public static class HttpRequestExtensions
{

	public static string GetDerivedComponentValue(this HttpRequest request, DerivedComponent derivedComponent)
	{
		string value = derivedComponent.ComponentName switch
		{
			Constants.DerivedComponents.SignatureParams
				=> throw new NotSupportedException("The '@signature-params' component value cannot be retrieved like this."),
			Constants.DerivedComponents.Method => request.Method,

			Constants.DerivedComponents.TargetUri =>
				$"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}",
			Constants.DerivedComponents.Authority => NormalizeAuthority(request),
			Constants.DerivedComponents.Scheme => request.Scheme.ToLower(),

			Constants.DerivedComponents.RequestTarget => $"{request.PathBase}{request.Path}{request.QueryString}",

			Constants.DerivedComponents.Path => $"{request.PathBase}{request.Path}",
			Constants.DerivedComponents.Query => request.QueryString.HasValue ? request.QueryString.Value! : "?",
			Constants.DerivedComponents.QueryParam
				=> throw new NotSupportedException("The '@query-param' component value cannot be retrieved like this."),
			Constants.DerivedComponents.Status
				=> throw new NotSupportedException("The '@status' component value cannot be retrieved for request messages."),

			_ => throw new NotSupportedException(
				$"Non-standard derived signature component '{derivedComponent.ComponentName}' cannot be retrieved."),
		};

		return value!;
	}

	/// <summary>
	/// Normalizes the value for the <c>@authority</c> derived component: default ports are omitted and host values
	/// are lower-cased.
	/// </summary>
	/// <param name="request">
	/// The <see cref="HttpRequest"/> defining the values to use for the <c>@authority</c> derived component value.
	/// </param>
	/// <returns>
	/// A string value representing the <c>@authority</c> derived component value for the message.
	/// </returns>
	private static string NormalizeAuthority(HttpRequest request)
	{
		string scheme = request.Scheme;
		HostString host = request.Host;

		if (host.Port.HasValue)
		{
			if ((StringComparer.OrdinalIgnoreCase.Equals("http", scheme) && host.Port.Value == 80) ||
			    (StringComparer.OrdinalIgnoreCase.Equals("https", scheme) && host.Port.Value == 443))
			{
				return host.Host.ToLower();
			}
		}

		return host.Value.ToLower();
	}
}