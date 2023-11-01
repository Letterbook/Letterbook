using NSign;
using NSign.Signatures;
using static NSign.Constants;

namespace Letterbook.Adapter.ActivityPub.Signatures;

internal static class HttpRequestMessageExtensions
{
    public static string GetDerivedComponentValue(this HttpRequestMessage request, DerivedComponent derivedComponent)
    {
        return derivedComponent.ComponentName switch
        {
            DerivedComponents.SignatureParams =>
                throw new NotSupportedException("The '@signature-params' component cannot be included explicitly."),
            DerivedComponents.Method => request.Method.Method,
            DerivedComponents.TargetUri => request.RequestUri.OriginalString,
            DerivedComponents.Authority => request.RequestUri.Authority.ToLower(),
            DerivedComponents.Scheme => request.RequestUri.Scheme.ToLower(),
            DerivedComponents.RequestTarget => request.RequestUri.PathAndQuery,
            DerivedComponents.Path => request.RequestUri.AbsolutePath,
            DerivedComponents.Query =>
                String.IsNullOrWhiteSpace(request.RequestUri.Query) ? "?" : request.RequestUri.Query,
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