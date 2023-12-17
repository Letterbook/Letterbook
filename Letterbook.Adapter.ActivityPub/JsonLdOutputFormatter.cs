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
    
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var httpContext = context.HttpContext;
        var provider = httpContext.RequestServices;
        var logger = provider.GetRequiredService<ILogger<JsonLdOutputFormatter>>();

        if (context.Object is not ASObject asObject)
        {
            logger.LogError("Cannot serialize unrecognized object type {Type}", context.Object?.GetType());
            logger.LogDebug("Unknown object details {@Object}", context.Object);
            throw new NotSupportedException("Cannot serialize response");
        }
        
        var serializer = provider.GetRequiredService<IJsonLdSerializer>();
        var json = serializer.Serialize(asObject);
        logger.LogDebug("Writing response body {Json}", json);
        await httpContext.Response.WriteAsync(json); //.WriteAsync(json, selectedEncoding);
    }
}