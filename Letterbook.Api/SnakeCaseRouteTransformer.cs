using System.Text.RegularExpressions;

namespace Letterbook.Api;

/// <summary>
/// Adjusts routes to adhere to mastodon's snake_cased url conventions
/// </summary>
public partial class SnakeCaseRouteTransformer : IOutboundParameterTransformer
{
    /// <summary>
    /// Matches on all capital letters in a string
    /// This is used to split PascalCased methods into word tokens
    /// i.e "PascalCasedMethod" => ["Pascal", "Cased", "Method"]
    /// 
    /// Note that multiple capitals in sequence will also split into tokens.
    /// i.e. "CPUInfo" => ["C", "P", "U", "Info"]
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("(?<!^)(?=[A-Z])")]
    private static partial Regex RouteRegex();
    
    public string? TransformOutbound(object? value)
    {
        return value != null
            ? string.Join("_", RouteRegex().Split(value.ToString()!)).ToLower()
            : null;
    }
}