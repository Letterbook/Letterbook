using Microsoft.AspNetCore.Authentication.Cookies;
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
}