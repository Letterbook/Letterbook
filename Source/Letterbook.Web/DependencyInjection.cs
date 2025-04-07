using Medo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web;

public static class DependencyInjection
{
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
		// options.Conventions.AddPageRoute("/Profile", "/@{handle}");
		// options.Conventions.AddPageRoute("/Profile", "/@{handle}@{host}");
		// options.Conventions.AddPageRoute("/Profile", "/profile/{id}");
	}
}