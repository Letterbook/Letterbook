using Microsoft.AspNetCore.Rewrite;

namespace Letterbook.Docs;

public class RewriteRules
{
	public static void ReWriteRequests(RewriteContext context)
	{
		var request = context.HttpContext.Request;

		if (request.Path.Value?.EndsWith(".html", StringComparison.OrdinalIgnoreCase) == true)
		{
			context.HttpContext.Request.Path = context.HttpContext.Request.Path.Value?.Replace(".html","");

		}
	}

}