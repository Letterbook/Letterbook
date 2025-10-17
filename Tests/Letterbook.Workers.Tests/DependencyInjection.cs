using System.Text.Json;
using ActivityPub.Types;
using ActivityPub.Types.Conversion;
using Letterbook.Adapter.ActivityPub;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Authorization;
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
		return services
			// Configure AP docs to serialize with multiple lines for readability
			.Configure<JsonLdSerializerOptions>(options =>
			{
				options.DefaultJsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
			})
			.AddScoped<IActivityPubClient>(_ => mocks.ActivityPubClientMock.Object)
			.AddScoped<ITimelineService>(_ => mocks.TimelineServiceMock.Object)
			.AddScoped<IProfileService>(_ => mocks.ProfileServiceMock.Object)
			.AddScoped<IActivityScheduler>(_ => mocks.ActivityPublisherMock.Object)
			.AddScoped<IDataAdapter>(_ => mocks.DataAdapterMock.Object)
			.AddScoped<IActivityPubDocument, Document>()
			.AddSingleton<Instrumentation>()
			.AddSingleton<MappingConfigProvider>()
			.AddSingleton<IAuthorizationService, AuthorizationService>()
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