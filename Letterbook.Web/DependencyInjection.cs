using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Letterbook.Web;

public static class DependencyInjection
{
	public static AuthorizationOptions AddWebAuthzPolicy(this AuthorizationOptions options)
	{
		options.AddPolicy(Constants.AuthzPolicy, policy =>
		{
			policy.RequireAuthenticatedUser()
				.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme);
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
}