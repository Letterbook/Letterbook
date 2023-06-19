using System.Text.RegularExpressions;

namespace Letterbook.Api;

/// <summary>
/// Adjusts routes to adhere to mastodon's snake_cased url conventions
/// </summary>
public partial class RouteTransformer : IOutboundParameterTransformer
{
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex RouteRegex();
    
    public string? TransformOutbound(object? value)
    {
        return value != null
            ? RouteRegex().Replace(value.ToString(), "$1_$2").ToLower()
            : null;
    }
}