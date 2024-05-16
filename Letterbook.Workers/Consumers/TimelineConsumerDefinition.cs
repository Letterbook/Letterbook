using Letterbook.Workers.Consumers;

namespace Company.Consumers
{
    using MassTransit;

    public class TimelineConsumerDefinition :
        ConsumerDefinition<TimelineConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TimelineConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}