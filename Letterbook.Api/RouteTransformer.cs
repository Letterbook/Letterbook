using System.Text.RegularExpressions;

namespace Letterbook.Api;

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