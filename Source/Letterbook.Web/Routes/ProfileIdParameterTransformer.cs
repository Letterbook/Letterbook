namespace Letterbook.Web.Routes;

public class ProfileIdParameterTransformer : IOutboundParameterTransformer
{
	public string? TransformOutbound(object? value)
	{
		return value switch
		{
			Models.ProfileId p => p.ToString(),
			string s => s,
			_ => null
		};
	}
}