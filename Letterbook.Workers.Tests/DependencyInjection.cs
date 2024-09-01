using ActivityPub.Types;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Letterbook.Workers.Tests;

public static class DependencyInjection
{
	internal static IServiceCollection AddMocks(this IServiceCollection services, WithMocks mocks)
	{
		services.TryAddTypesModule();
		return services.AddScoped<IActivityPubClient>(_ => mocks.ActivityPubClientMock.Object)
			.AddScoped<ITimelineService>(_ => mocks.TimelineServiceMock.Object)
			.AddScoped<IProfileService>(_ => mocks.ProfileServiceMock.Object)
			.AddScoped<IActivityMessagePublisher>(_ => mocks.ActivityPublisherMock.Object)
			.AddScoped<IPostAdapter>(_ => mocks.PostAdapterMock.Object)
			.AddSingleton<MappingConfigProvider>()
			.AddScoped<IActivityPubDocument, Document>()
			.AddSingleton<IOptions<CoreOptions>>(mocks.CoreOptionsMock);
	}

	public static IBusRegistrationConfigurator AddTestBus(this IBusRegistrationConfigurator bus)
	{
		bus.UsingInMemory((ctx, cfg) =>
		{
			cfg.UseDelayedMessageScheduler();
			cfg.ConfigureJsonSerializerOptions(options => options.AddDtoSerializer());
			cfg.ConfigureEndpoints(ctx);
		});

		return bus;
	}
}