using System.Text;
using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.ActivityPub;

public class JsonLdInputFormatter : TextInputFormatter
{
    public JsonLdInputFormatter()
    {
        SupportedMediaTypes.Add("application/ld+json");
        SupportedMediaTypes.Add("application/activity+json");
        SupportedMediaTypes.Add(@"application/ld+json; profile=""https://www.w3.org/ns/activitystreams""");
        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanReadType(Type type) => typeof(ASType).IsAssignableFrom(type);

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context,
        Encoding encoding)
    {
        var httpContext = context.HttpContext;
        var provider = httpContext.RequestServices;
        var logger = provider.GetRequiredService<ILogger<JsonLdInputFormatter>>();

        var serializer = provider.GetRequiredService<IJsonLdSerializer>();
        var result = serializer.DeserializeAsync<ASType>(httpContext.Request.Body);

        await LogActivity(httpContext.Request.Body, logger);
        return await InputFormatterResult.SuccessAsync(await result);
    }

    private async ValueTask LogActivity(Stream body, ILogger logger)
    {
        if (!logger.IsEnabled(LogLevel.Debug) || !body.CanRead) return;

        try
        {
            body.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[body.Length];
            var read = await body.ReadAsync(buffer, 0, (int)body.Length);

            if (read > 0)
            {
                logger.LogDebug("Raw activity {Json}", Encoding.UTF8.GetString(buffer));
            }
            else logger.LogDebug("Couldn't reread the request body");
        }
        catch (Exception e)
        {
            logger.LogError("Error logging raw Activity {Error}", e);
        }
    }
}