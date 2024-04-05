using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Adapter.ActivityPub.Signatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NSign;
using NSign.Http;
using NSign.Providers;
using NSign.Signatures;

namespace Letterbook.Api.Authentication.HttpSignature;

public interface IFederatedActorHttpSignatureVerifier
{
	Task<bool> VerifyAsync(
		HttpContext context,
		CancellationToken cancellationToken);
}

[UsedImplicitly]
public class FederatedActorHttpSignatureVerifier(
	ILoggerFactory loggerFactory,
	IKeyMaterialProvider keyMaterialProvider) : IFederatedActorHttpSignatureVerifier
{
	private static readonly HttpFieldOptions HttpFieldOptions = new();

	private readonly ILogger _logger = loggerFactory.CreateLogger<FederatedActorHttpSignatureVerifier>();

	public async Task<bool> VerifyAsync(
		HttpContext context,
		CancellationToken cancellationToken)
	{
		try
		{
			var signingContext = new HttpMessageSigningContext(_logger, HttpFieldOptions, context);
			if (signingContext.HasSignaturesForVerification)
			{
				return await VerifyRfcSignature(signingContext, cancellationToken);
			}

			return await VerifyMastodonSignature(context.Request, cancellationToken);
		}
		catch (SignatureVerificationException)
		{
			return false;
		}
	}

	private async Task<bool> VerifyMastodonSignature(HttpRequest request, CancellationToken cancellationToken)
	{
		try
		{
			var mastodonVerifier = new MastodonVerifier(loggerFactory.CreateLogger<MastodonVerifier>());
			var signatureComponents = mastodonVerifier
				.ParseMastodonSignatureComponents(request)
				.ToList();
			if (!signatureComponents.Any())
			{
				return false;
			}

			foreach (var signatureComponent in signatureComponents)
			{
				var key = await GetKey(signatureComponent.KeyId, cancellationToken);

			}

			return true;
		}
		catch (VerifierException)
		{
			// TODO: I didn't want to mess with the verifier interface too much yet, but we don't want to throw exceptions
			// for every unsigned request.
			return false;
		}
	}

	private async Task<bool> VerifyRfcSignature(HttpMessageSigningContext signingContext, CancellationToken cancellationToken)
	{
		foreach (var signatureContext in signingContext.SignaturesForVerification)
		{
			var signatureParams = signatureContext.SignatureParams;
			var key = await GetKey(signatureParams.KeyId, cancellationToken);

			var verifier = GetVerifier(signatureParams.Algorithm, signatureParams.KeyId, key);
			var input = signingContext.GetSignatureInput(signatureParams, out _);
			var verificationResult = await verifier.VerifyAsync(
				signatureParams,
				input,
				signatureContext.Signature,
				cancellationToken);

			if (verificationResult is not VerificationResult.SuccessfullyVerified)
			{
				_logger.LogWarning("HTTP request signature validation failed");
				return false;
			}
		}

		_logger.LogInformation("HTTP request signature validation succeeded");
		return true;
	}

	private async Task<X509Certificate2> GetKey(string? keyId, CancellationToken cancellationToken)
	{
		var key = await keyMaterialProvider.GetKeyByIdAsync(keyId, cancellationToken);
		if (key == null)
		{
			throw new SignatureVerificationException(
				$"Unable to verify signature: key with ID {keyId} was not found");
		}
		return key;
	}

	private IVerifier GetVerifier(string? algorithm, string? keyId, X509Certificate2 key)
	{
		var publicKey = key.GetRSAPublicKey();
		if (publicKey == null)
		{
			throw new SignatureVerificationException($"No RSA public key for {keyId}");
		}

		return new RsaPssSha512SignatureProvider(null, publicKey, keyId);
	}
}

public class HttpMessageSigningContext(ILogger logger, HttpFieldOptions options, HttpContext httpContext)
	: MessageContext(logger, options)
{
	/// <inheritdoc/>
        public override SignatureVerificationOptions? VerificationOptions => null;

        /// <inheritdoc/>
        public override bool HasResponse => false;

        /// <inheritdoc/>
        public sealed override CancellationToken Aborted => httpContext.RequestAborted;

        /// <inheritdoc/>
        public override void AddHeader(string headerName, string value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override string GetDerivedComponentValue(DerivedComponent component)
        {
            return httpContext.Request.GetDerivedComponentValue(component);
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<string> GetHeaderValues(string headerName)
        {
            if (TryGetHeaderValues(httpContext.Request.Headers, headerName, out StringValues values))
            {
                return values;
            }

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<string> GetRequestHeaderValues(string headerName)
        {
            if (TryGetHeaderValues(httpContext.Request.Headers, headerName, out StringValues values))
            {
                return values;
            }

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetTrailerValues(string fieldName)
        {
            // This may still throw a NotSupportedException when trailers are not supported, or a InvalidOperationException
            // when trailers are not yet available. For now, let's bubble them up.
            if (TryGetHeaderValues(RequestTrailers, fieldName, out StringValues values))
            {
                return values;
            }

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetRequestTrailerValues(string fieldName)
        {
            // This may still throw a NotSupportedException when trailers are not supported, or a InvalidOperationException
            // when trailers are not yet available. For now, let's bubble them up.
            if (TryGetHeaderValues(RequestTrailers, fieldName, out StringValues values))
            {
                return values;
            }

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc/>
        public sealed override IEnumerable<string> GetQueryParamValues(string paramName)
        {
            if (httpContext.Request.Query.TryGetValue(paramName, out StringValues values))
            {
                return values;
            }

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc/>
        public sealed override bool HasHeader(bool bindRequest, string headerName)
        {
            IHeaderDictionary headers;

            if (!HasResponse || bindRequest)
            {
                headers = httpContext.Request.Headers;
            }
            else // if (HasResponse && !bindRequest)
            {
                headers = httpContext.Response.Headers;
            }


            return TryGetHeaderValues(headers, headerName, out _);
        }

        /// <inheritdoc/>
        public override bool HasTrailer(bool bindRequest, string fieldName)
        {
	        return httpContext.Response.SupportsTrailers() && TryGetHeaderValues(RequestTrailers, fieldName, out _);
        }

        /// <inheritdoc/>
        public sealed override bool HasExactlyOneQueryParamValue(string paramName)
        {
            return httpContext.Request.Query.TryGetValue(paramName, out StringValues values)
                && values.Count == 1;
        }

        /// <summary>
        /// Tries to get the message header values for the header with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="headers">
        /// The <see cref="IHeaderDictionary"/> in which to look for the header.
        /// </param>
        /// <param name="name">
        /// The name of the header to get the values for.
        /// </param>
        /// <param name="values">
        /// If the header exists, is updated with the values of the header.
        /// </param>
        /// <returns>
        /// True if the header exists, or false otherwise.
        /// </returns>
        /// <remarks>
        /// This method also emulates synthetic headers like 'content-length' which are in some cases calculated based
        /// on the content.
        /// </remarks>
        private static bool TryGetHeaderValues(IHeaderDictionary headers, string name, out StringValues values)
        {
            if (headers.TryGetValue(name, out values))
            {
                return true;
            }

            switch (name)
            {
                case "content-length":
                    if (headers.ContentLength.HasValue)
                    {
                        values = new StringValues(headers.ContentLength.Value.ToString());
                        return true;
                    }
                    break;
            }

            return false;
        }
        /// <summary>
        /// Gets an <see cref="IHeaderDictionary"/> representing the trailers of the request of the context.
        /// </summary>
        private IHeaderDictionary RequestTrailers
        {
            get
            {
                IHttpRequestTrailersFeature? feature = httpContext.Features.Get<IHttpRequestTrailersFeature>();
                if (null == feature)
                {
                    throw new NotSupportedException("This request does not support trailers.");
                }

                return feature.Trailers;
            }
        }
}

public static class HttpRequestExtensions
{

	public static string GetDerivedComponentValue(this HttpRequest request, DerivedComponent derivedComponent)
	{
		string value = derivedComponent.ComponentName switch
		{
			Constants.DerivedComponents.SignatureParams
				=> throw new NotSupportedException("The '@signature-params' component value cannot be retrieved like this."),
			Constants.DerivedComponents.Method => request.Method,
			// TODO: Need to figure out a way to deal with reverse proxies changing paths, i.e. getting the original path/prefix.
			Constants.DerivedComponents.TargetUri =>
				$"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}",
			Constants.DerivedComponents.Authority => NormalizeAuthority(request),
			Constants.DerivedComponents.Scheme => request.Scheme.ToLower(),
			// TODO: Need to figure out a way to deal with reverse proxies changing paths, i.e. getting the original path/prefix.
			Constants.DerivedComponents.RequestTarget => $"{request.PathBase}{request.Path}{request.QueryString}",
			// TODO: Need to figure out a way to deal with reverse proxies changing paths, i.e. getting the original path/prefix.
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