using Letterbook.Web.Mappers;
using Letterbook.Web.Routes;
using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web;

public static class DependencyInjection
{
	public static IServiceCollection AddWebServices(this IServiceCollection services)
	{
		return services.AddSingleton<FormsProfileProvider>();
	}

	public static AuthorizationOptions AddWebAuthzPolicy(this AuthorizationOptions options)
	{
		options.AddPolicy(Constants.AuthzPolicy, policy =>
		{
			policy.RequireAuthenticatedUser();
		});
		return options;
	}

	public static IServiceCollection AddWebCookies(this IServiceCollection services)
	{
		return services.ConfigureApplicationCookie(options =>
		{
			options.SlidingExpiration = true;
			options.ExpireTimeSpan = TimeSpan.FromDays(90);
		});
	}

	public static void AddWebRoutes(this RazorPagesOptions options)
	{
		options.Conventions.Add(new PageRouteTransformerConvention(new ProfileHandleParameterTransformer()));
		options.Conventions.Add(new PageRouteTransformerConvention(new ProfileIdParameterTransformer()));
		options.Conventions.AddPageRoute("/Profile", "/{id:handle}");
	}

	public static void ConfigureWebRoutes(this RouteOptions options)
	{
		options.ConstraintMap.Add("handle", typeof(ProfileHandleParameterTransformer));
		options.LowercaseUrls = true;
	}
}