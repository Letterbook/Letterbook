using System.Text;
using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.ActivityPub;

public class JsonLdOutputFormatter : TextOutputFormatter
{
	public JsonLdOutputFormatter()
	{
		SupportedMediaTypes.Add("application/ld+json");
		SupportedMediaTypes.Add("application/activity+json");
		SupportedMediaTypes.Add(@"application/ld+json; profile=""https://www.w3.org/ns/activitystreams""");
		SupportedEncodings.Add(Encoding.UTF8);
	}

	protected override bool CanWriteType(Type? type) => typeof(ASType).IsAssignableFrom(type);

	public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
	{
		var httpContext = context.HttpContext;
		var provider = httpContext.RequestServices;
		var logger = provider.GetRequiredService<ILogger<JsonLdOutputFormatter>>();

		var serializer = provider.GetRequiredService<IJsonLdSerializer>();
		var json = serializer.Serialize(context.Object);
		logger.LogDebug("Writing response body {Json}", json);
		return httpContext.Response.WriteAsync(json);
	}
}