using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using JsonOptions = Letterbook.ActivityPub.JsonOptions;

namespace Letterbook.Api;

public class JsonLdSerializerAttribute : ActionFilterAttribute 
{
    public override void OnActionExecuted(ActionExecutedContext ctx)
    {
        if (ctx.Result is OkObjectResult objectResult)
        {
            objectResult.Formatters.Clear();
            objectResult.Formatters.Add(new SystemTextJsonOutputFormatter(JsonOptions.ActivityPub));
            objectResult.ContentTypes.Clear();
            objectResult.ContentTypes.Add("application/ld+json");
        }
    }
}