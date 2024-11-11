using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Letterbook.Api.Swagger;

public class RequiredHeaders : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var contentTypes =
			context.ApiDescription.ActionDescriptor.ActionConstraints?
				.Where(c => c is AcceptHeaderAttribute)
				.SelectMany(attr => (attr as AcceptHeaderAttribute)?.ContentTypes!)
				.Where(x => !string.IsNullOrEmpty(x))
				.ToList();
		if (contentTypes?.Any() == true)
		{
			operation.Parameters ??= new List<OpenApiParameter>();
			var schema = new OpenApiSchema()
			{
				Default = new OpenApiString(contentTypes.First()),
				AnyOf = contentTypes.Select(t => new OpenApiSchema()
				{
					Type = "string",
					Default = new OpenApiString(t)
				}).ToList()
			};
			var mediaType = new OpenApiMediaType()
			{
				Schema = schema
			};
			operation.Parameters.Add(new OpenApiParameter
			{
				Name = "Accept",
				In = ParameterLocation.Header,
				// Content = new Dictionary<string, OpenApiMediaType>(){{contentTypes.First(), mediaType}},
				Description = "Media Type",
				Required = true,
				Schema = new OpenApiSchema()
				{
					Default = new OpenApiString(contentTypes.First()),
					AnyOf = contentTypes.Select(t => new OpenApiSchema()
					{
						Type = "string",
						Default = new OpenApiString(t)
					}).ToList()
				}
			});
		}
	}
}