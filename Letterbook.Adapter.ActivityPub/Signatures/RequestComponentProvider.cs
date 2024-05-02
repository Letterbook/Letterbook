using System.Diagnostics.CodeAnalysis;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using NSign;
using NSign.Signatures;
using static NSign.Constants;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public abstract class RequestComponentProvider
{
	public abstract bool TryGetHeaderValues(string key, [NotNullWhen(true)] out IEnumerable<string>? values);
	public abstract Dictionary<string, IList<string>> GetAllHeaders();

	protected abstract Uri? GetRequestUri();
	protected abstract string GetHttpMethod();

	public string GetDerivedComponentValue(DerivedComponent derivedComponent)
	{
		if (GetRequestUri() is not { } uri)
			throw ClientException.SignatureError();

		return derivedComponent.ComponentName switch
		{
			DerivedComponents.SignatureParams =>
				throw new NotSupportedException("The '@signature-params' component cannot be included explicitly."),
			DerivedComponents.Method => GetHttpMethod(),
			DerivedComponents.TargetUri => uri.OriginalString,
			DerivedComponents.Authority => uri.Authority.ToLower(),
			DerivedComponents.Scheme => uri.Scheme.ToLower(),
			DerivedComponents.RequestTarget => uri.PathAndQuery,
			DerivedComponents.Path => uri.AbsolutePath,
			DerivedComponents.Query =>
				String.IsNullOrWhiteSpace(uri.Query) ? "?" : uri.Query,
			DerivedComponents.QueryParam =>
				throw new NotSupportedException("The '@query-param' component must have the 'name' parameter set."),
			DerivedComponents.Status =>
				throw new NotSupportedException("The '@status' component cannot be included in request signatures."),

			_ =>
				throw new NotSupportedException(
					$"Non-standard derived signature component '{derivedComponent.ComponentName}' cannot be retrieved."),
		};
	}
}

public class HttpRequestMessageComponentProvider(HttpRequestMessage message) : RequestComponentProvider
{
	public override bool TryGetHeaderValues(string key, [NotNullWhen(true)] out IEnumerable<string>? values)
	{
		if (message.Headers.TryGetValues(key, out values))
		{
			return true;
		}

		if (message.Content == null)
		{
			values = null;
			return false;
		}

		return message.Content.Headers.TryGetValues(key, out values);
	}

	public override Dictionary<string, IList<string>> GetAllHeaders()
	{
		return message.Headers
			.ToDictionary(
				h => h.Key,
				h => (IList<string>)h.Value.ToList());
	}

	protected override Uri? GetRequestUri()
	{
		return message.RequestUri;
	}

	protected override string GetHttpMethod()
	{
		return message.Method.Method;
	}
}

public class HttpRequestComponentProvider(HttpRequest request) : RequestComponentProvider
{
	public override bool TryGetHeaderValues(string key, [NotNullWhen(true)] out IEnumerable<string>? values)
	{
		if (request.Headers.TryGetValue(key, out var stringValues))
		{
			values = stringValues.WhereNotNull();
			return true;
		}

		values = null;
		return false;
	}

	public override Dictionary<string, IList<string>> GetAllHeaders()
	{
		return request.Headers
			.ToDictionary(
				h => h.Key,
				h => (IList<string>)h.Value.ToList());
	}

	protected override Uri? GetRequestUri()
	{
		var httpContext = request.HttpContext;

		var requestFeature = httpContext.Features.Get<IHttpRequestFeature>();
		if (requestFeature == null)
		{
			return null;
		}

		return new Uri(requestFeature.RawTarget);
	}

	protected override string GetHttpMethod()
	{
		return request.Method;
	}
}
