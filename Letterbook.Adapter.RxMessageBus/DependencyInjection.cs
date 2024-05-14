using Letterbook.Core.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Adapter.RxMessageBus;

public static class DependencyInjection
{
	public static IServiceCollection AddRxMessageBus(this IServiceCollection services)
	{
		services.AddSingleton<IRxMessageChannels, RxMessageChannels>();
		services.AddSingleton<IMessageBusAdapter, RxMessageBus>();
		services.AddSingleton<IMessageBusClient, RxMessageBus>();

		return services;
	}
}