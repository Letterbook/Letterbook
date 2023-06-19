using System.Text.RegularExpressions;

namespace Letterbook.Api;

/// <summary>
/// Adjusts routes to adhere to mastodon's snake_cased url conventions
/// </summary>
public partial class RouteTransformer : IOutboundParameterTransformer
{
    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex RouteRegex();
    
    public string? TransformOutbound(object? value)
    {
        return value != null
            ? string.Join("_", RouteRegex().Split(value.ToString())).ToLower()
            : null;
    }
}