namespace Letterbook.Web.Routes;

public class ProfileHandleParameterTransformer : IOutboundParameterTransformer
{
	public string? TransformOutbound(object? value)
	{
		return value == null ? null : $"@{value}";
	}
}