using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using DarkLink.Web.WebFinger.Server;
using DarkLink.Web.WebFinger.Shared;

namespace Letterbook.Api;

public static class DependencyInjectionExtensions
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		Converters = { new JsonResourceDescriptorConverter(), },
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	public static IServiceCollection AddWebfinger(this IServiceCollection services)
	{
		return services.AddScoped<IResourceDescriptorProvider, WebfingerProvider>();
	}

	public static void UseWebFingerScoped(this IApplicationBuilder app)
	{
		app.Map(
			Constants.HTTP_PATH,
			app => app.Run(async ctx =>
			{
				if (!ctx.Request.Query.TryGetValue(Constants.QUERY_RESOURCE, out var resourceRaw)
					|| !Uri.TryCreate(resourceRaw, UriKind.RelativeOrAbsolute, out var resource))
				{
					ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
					return;
				}

				using var scope = app.ApplicationServices.CreateScope();
				var resourceDescriptorProvider =
					scope.ServiceProvider.GetRequiredService<IResourceDescriptorProvider>();

				ctx.Request.Query.TryGetValue(Constants.QUERY_RELATION, out var relations);

				var descriptor = await resourceDescriptorProvider.GetResourceDescriptorAsync(resource, relations, ctx.Request, ctx.RequestAborted);
				if (descriptor is null)
				{
					ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
					return;
				}

				await ctx.Response.WriteAsJsonAsync(
					descriptor,
					JsonSerializerOptions,
					Constants.MEDIA_TYPE,
					ctx.RequestAborted);
			}));
	}

}