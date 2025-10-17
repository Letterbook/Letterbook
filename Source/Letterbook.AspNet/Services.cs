using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.AspNet;

public static class Services
{
	public static IServiceCollection AddAspnetServices(this IServiceCollection services)
	{
		services.AddScoped<AuthorizePeerService>();

		return services;
	}

}