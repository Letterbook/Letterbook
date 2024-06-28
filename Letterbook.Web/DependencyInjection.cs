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

	public static IServiceCollection AddWebAuthz(this IServiceCollection services, ConfigurationManager configuration)
	{
		services.AddAuthorization(options =>
		{
			options.AddWebAuthzPolicy();
		});

		return services;
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