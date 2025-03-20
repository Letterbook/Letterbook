using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NSign;
using NSign.Http;
using NSign.Signatures;

namespace Letterbook.Api.Authentication.HttpSignature.Verification;

/// <summary>
/// A message signing context that allows verification of signatures in a <see cref="HttpRequest"/>. This class is copied and
/// adapted from the NSign implementation for <see cref="HttpRequestMessage"/>.
/// </summary>
public class AspNetCoreMessageSigningContext(ILogger logger, HttpFieldOptions options, HttpContext httpContext)
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