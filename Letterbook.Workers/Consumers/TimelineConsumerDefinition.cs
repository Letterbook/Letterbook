using MassTransit;

namespace Letterbook.Workers.Consumers
{
	public class TimelineConsumerDefinition :
        ConsumerDefinition<TimelineConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpoint,
	        IConsumerConfigurator<TimelineConsumer> consumer,
	        IRegistrationContext context)
        {
	        endpoint.UseMessageRetry(r => r.Intervals(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(1000)));
        }
    }
}