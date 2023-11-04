using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Letterbook.Api;

/// <summary>
/// AspNet doesn't have this kind of content type filtering built-in, for some reason. I guess they expect everything
/// to be handled by content negotiation? That doesn't work for us, because we would return different content, not just
/// different views of it.
/// Anyway, this should allow controllers to handle only activitypub requests.
/// That way we can handle regular web requests with their own controller.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AcceptHeaderAttribute : Attribute, IActionConstraint
{
    public int Order => 0;
    public MediaTypeCollection ContentTypes { get; set; }
    
    public AcceptHeaderAttribute(string contentType, params string[] otherContentTypes)
    {            
        if (contentType == null)
            throw new ArgumentNullException(nameof(contentType));

        // We want to ensure that the given provided content types are valid values, so
        // we validate them using the semantics of MediaTypeHeaderValue.
        MediaTypeHeaderValue.Parse(contentType);

        for (var i = 0; i < otherContentTypes.Length; i++)
        {
            MediaTypeHeaderValue.Parse(otherContentTypes[i]);
        }

        ContentTypes = GetContentTypes(contentType, otherContentTypes);
    }

    
    public bool Accept(ActionConstraintContext context)
    {
        var acceptHeader = context.RouteContext.HttpContext.Request.Headers[HeaderNames.Accept];

        return IsMatch(acceptHeader);
    }

    public bool IsMatch(string? acceptHeader)
    {
        if (string.IsNullOrEmpty(acceptHeader)) return false;
        var acceptTypes = acceptHeader.Split(",").Select(each => new MediaType(each));
        return acceptTypes.Aggregate(false,
            (result, acceptType) => result || ContentTypes.Aggregate(false,
                (contentResult, contentType) => contentResult || acceptType.IsSubsetOf(new MediaType(contentType))));
    }
    
    private MediaTypeCollection GetContentTypes(string firstArg, string[] args)
    {
        var completeArgs = new List<string>();
        completeArgs.Add(firstArg);
        completeArgs.AddRange(args);

        var contentTypes = new MediaTypeCollection();
        foreach (var arg in completeArgs)
        {
            contentTypes.Add(arg);
        }

        return contentTypes;
    }
}