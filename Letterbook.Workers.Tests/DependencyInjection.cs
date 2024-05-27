using ActivityPub.Types;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Letterbook.Workers.Tests;

public static class DependencyInjection
{
	public static IServiceCollection AddMocks(this IServiceCollection services, WithMocks mocks)
	{
		services.TryAddTypesModule();
		return services.AddScoped<IActivityPubClient>(_ => mocks.ActivityPubClientMock.Object)
			.AddScoped<ITimelineService>(_ => mocks.TimelineServiceMock.Object)
			.AddScoped<IProfileService>(_ => mocks.ProfileServiceMock.Object)
			.AddSingleton<IOptions<CoreOptions>>(mocks.CoreOptionsMock);
	}
}