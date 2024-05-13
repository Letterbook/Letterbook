using MassTransit;
// using MassTransit.MessageRetryConfigurationExtensions;

namespace Letterbook.Core.Workers;

public class DeliveryWorkerDefinition : ConsumerDefinition<DeliveryWorker>
{
	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpoint, IConsumerConfigurator<DeliveryWorker> consumer, IRegistrationContext registration)
	{
		// endpoint.Use
		// endpoint.Use
		// endpoint.UseMessageRetry(r => r.Intervals(500, 1000));
	}
}