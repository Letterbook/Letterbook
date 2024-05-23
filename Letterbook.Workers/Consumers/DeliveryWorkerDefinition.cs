using MassTransit;

namespace Letterbook.Workers.Consumers;

public class DeliveryWorkerDefinition : ConsumerDefinition<DeliveryWorker>
{
	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
		IConsumerConfigurator<DeliveryWorker> consumerConfigurator,
		IRegistrationContext context)
	{
		endpointConfigurator.UseMessageRetry(retry => retry.Incremental(4, TimeSpan.FromSeconds(3), TimeSpan.FromMinutes(1)));
	}
}