using NSign;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

internal static class HttpRequestMessageExtensions
{
    public static string GetDerivedComponentValue(this HttpRequestMessage request, DerivedComponent derivedComponent)
    {
        return derivedComponent.ComponentName switch
        {
            Constants.DerivedComponents.SignatureParams =>
                throw new NotSupportedException("The '@signature-params' component cannot be included explicitly."),
            Constants.DerivedComponents.Method => request.Method.Method,
            Constants.DerivedComponents.TargetUri => request.RequestUri.OriginalString,
            Constants.DerivedComponents.Authority => request.RequestUri.Authority.ToLower(),
            Constants.DerivedComponents.Scheme => request.RequestUri.Scheme.ToLower(),
            Constants.DerivedComponents.RequestTarget => request.RequestUri.PathAndQuery,
            Constants.DerivedComponents.Path => request.RequestUri.AbsolutePath,
            Constants.DerivedComponents.Query =>
                String.IsNullOrWhiteSpace(request.RequestUri.Query) ? "?" : request.RequestUri.Query,
            Constants.DerivedComponents.QueryParam =>
                throw new NotSupportedException("The '@query-param' component must have the 'name' parameter set."),
            Constants.DerivedComponents.Status =>
                throw new NotSupportedException("The '@status' component cannot be included in request signatures."),

            _ =>
                throw new NotSupportedException(
                    $"Non-standard derived signature component '{derivedComponent.ComponentName}' cannot be retrieved."),
        };
    }
}